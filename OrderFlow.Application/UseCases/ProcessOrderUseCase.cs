using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Messaging;
using OrderFlow.Application.Observability;
using OrderFlow.Domain.Interfaces;
using System.Diagnostics;

namespace OrderFlow.Application.UseCases
{
    public class ProcessOrderUseCase : IProcessOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessOrderUseCase> _logger;
        private readonly IOrderEventPublisher _orderEventPublisher;
        private readonly ICorrelationContext _correlationContext;
        private readonly IOrderProcessingStrategyResolver _strategyResolver;
        private readonly IRiskAnalysisGateway _riskAnalysisGateway;
        private readonly IOrderCacheService _orderCacheService;

        public ProcessOrderUseCase(
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork,
            ILogger<ProcessOrderUseCase> logger,
            IOrderEventPublisher orderEventPublisher,
            ICorrelationContext correlationContext,
            IOrderProcessingStrategyResolver strategyResolver,
            IRiskAnalysisGateway riskAnalysisGateway,
            IOrderCacheService orderCacheService)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _orderEventPublisher = orderEventPublisher;
            _correlationContext = correlationContext;
            _strategyResolver = strategyResolver;
            _riskAnalysisGateway = riskAnalysisGateway;
            _orderCacheService = orderCacheService;
        }

        public async Task ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

            if (order is null)
            {
                _logger.LogWarning("Ordem {OrderId} não encontrada para processamento", orderId);
                throw new InvalidOperationException($"Ordem {orderId} não encontrada.");
            }

            using var activity = Telemetry.ActivitySource.StartActivity("ProcessOrder");

            activity?.SetTag("order.id", order.Id);
            activity?.SetTag("order.type", order.Type.ToString());
            activity?.SetTag("order.amount", order.Amount);

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                [LogProperties.CorrelationId] = _correlationContext.CorrelationId,
                [LogProperties.OrderId] = order.Id,
                [LogProperties.UserId] = order.UserId,
                [LogProperties.OrderType] = order.Type,
                [LogProperties.Status] = order.Status,
                [LogProperties.ExternalReference] = order.ExternalReference
            }))
            {
                if (order is null)
                {
                    _logger.LogWarning("Ordem {OrderId} não encontrada para processamento", orderId);
                    throw new InvalidOperationException($"Ordem {orderId} não encontrada.");
                }

                if (!order.CanBeProcessed())
                {
                    _logger.LogWarning(
                        "{Event} - Ordem ignorada por idempotência. Status atual {Status}",
                        LogEvents.OrderProcessingIgnored,
                        order.Status);

                    return;
                }

                try
                {
                    _logger.LogInformation("{Event} - Iniciando processamento da ordem", LogEvents.OrderProcessingStarted);

                    //if (order.Amount > 1000)
                    //{
                    //    _logger.LogWarning(
                    //        "Simulando falha para testes de retry/DLQ. OrderId {OrderId}",
                    //        order.Id);

                    //    throw new Exception("Falha simulada para retry e DLQ.");
                    //}

                    order.MarkAsProcessing(order.Amount);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    await _orderCacheService.RemoveAsync(order.Id, cancellationToken);


                    _logger.LogInformation(
                        "Ordem {OrderId} movida para {Status}",
                        order.Id,
                        order.Status);
                }
                catch (DbUpdateConcurrencyException)
                {
                    Metrics.OrdersFailed.Add(1);
                    _logger.LogWarning(
                        "Conflito de concorrência ao mover ordem {OrderId} para Processing",
                        order.Id);

                    return;
                }
                catch (InvalidOperationException ex)
                {
                    Metrics.OrdersFailed.Add(1);

                    _logger.LogWarning(
                        ex,
                        "Transição inválida ao mover ordem {OrderId} para Processing",
                        order.Id);

                    return;
                }

                await Task.Delay(2000, cancellationToken);

                var strategy = _strategyResolver.Resolve(order.Type);

                await strategy.ProcessAsync(order, cancellationToken);

                var riskResult = await _riskAnalysisGateway.AnalyzeAsync(order, cancellationToken);

                if (!riskResult.Approved)
                {
                    _logger.LogWarning(
                        "Order {OrderId} rejected by risk analysis. Reason {Reason}",
                        order.Id,
                        riskResult.Reason);

                    order.MarkAsFailed(riskResult.Reason);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    await _orderCacheService.RemoveAsync(order.Id, cancellationToken);

                    return;
                }

                try
                {
                    order.MarkAsCompleted(order.Amount);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    await _orderCacheService.RemoveAsync(order.Id, cancellationToken);

                    var integrationEvent = new OrderCompletedIntegrationEvent
                    {
                        OrderId = order.Id,
                        UserId = order.UserId,
                        Amount = order.Amount,
                        CompletedAt = DateTime.UtcNow,
                        CorrelationId = _correlationContext.CorrelationId
                    };

                    _logger.LogInformation(
                        "{Event} - Ordem {OrderId} concluída com status {Status}",
                        LogEvents.OrderCompleted,
                        order.Id,
                        order.Status);

                    await _orderEventPublisher.PublishOrderCompletedAsync(integrationEvent, cancellationToken);

                    _logger.LogInformation(
                        "{Event} - Evento OrderCompleted publicado no Kafka",
                        LogEvents.KafkaEventPublished);
                }
                catch (DbUpdateConcurrencyException)
                {
                    Metrics.OrdersFailed.Add(1);

                    _logger.LogWarning(
                        "Conflito de concorrência ao concluir ordem {OrderId}",
                        order.Id);
                }
            }

            stopwatch.Stop();

            Metrics.OrderProcessingTime.Record(stopwatch.Elapsed.TotalMilliseconds);
            Metrics.OrdersProcessed.Add(1);
        }
    }
}