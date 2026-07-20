using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Messaging;
using OrderFlow.Application.Observability;
using OrderFlow.Domain.Entities;
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

            using var activity = Telemetry.ActivitySource.StartActivity("ProcessOrder");

            activity?.SetTag("order.id", orderId);
            activity?.SetTag("correlation.id", _correlationContext.CorrelationId);

            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

                if (order is null)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, "Order not found");

                    _logger.LogWarning(
                        "Ordem {OrderId} não encontrada para processamento",
                        orderId);

                    throw new InvalidOperationException($"Ordem {orderId} não encontrada.");
                }

                activity?.SetTag("order.userId", order.UserId);
                activity?.SetTag("order.type", order.Type.ToString());
                activity?.SetTag("order.amount", order.Amount);
                activity?.SetTag("order.status", order.Status.ToString());

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
                    if (!order.CanBeProcessed())
                    {
                        activity?.SetTag("processing.ignored", true);
                        activity?.SetTag("processing.ignore.reason", "Order cannot be processed");
                        activity?.SetStatus(ActivityStatusCode.Ok);

                        _logger.LogWarning(
                            "{Event} - Ordem ignorada por idempotência. Status atual {Status}",
                            LogEvents.OrderProcessingIgnored,
                            order.Status);

                        return;
                    }

                    try
                    {
                        _logger.LogInformation(
                            "{Event} - Iniciando processamento da ordem",
                            LogEvents.OrderProcessingStarted);

                        var previousStatus = order.Status.ToString();

                        order.MarkAsProcessing(order.Amount);

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        await _orderCacheService.RemoveAsync(order.Id, cancellationToken);

                        await PublishStatusChangedAsync(
                            order,
                            previousStatus,
                            reason: null,
                            cancellationToken);

                        activity?.SetTag("order.status.after.processing", order.Status.ToString());

                        _logger.LogInformation(
                            "Ordem {OrderId} movida para {Status}",
                            order.Id,
                            order.Status);
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        Metrics.OrdersFailed.Add(1);

                        activity?.SetStatus(
                            ActivityStatusCode.Error,
                            "Concurrency conflict while moving order to Processing");

                        activity?.AddException(ex);

                        _logger.LogWarning(
                            ex,
                            "Conflito de concorrência ao mover ordem {OrderId} para Processing",
                            order.Id);

                        return;
                    }
                    catch (InvalidOperationException ex)
                    {
                        Metrics.OrdersFailed.Add(1);

                        activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                        activity?.AddException(ex);

                        _logger.LogWarning(
                            ex,
                            "Transição inválida ao mover ordem {OrderId} para Processing",
                            order.Id);

                        return;
                    }

                    // await Task.Delay(2000, cancellationToken);

                    var strategy = _strategyResolver.Resolve(order.Type);

                    activity?.SetTag("processing.strategy", strategy.GetType().Name);

                    await strategy.ProcessAsync(order, cancellationToken);

                    var riskResult = await _riskAnalysisGateway.AnalyzeAsync(order, cancellationToken);

                    activity?.SetTag("risk.approved", riskResult.Approved);

                    if (!riskResult.Approved)
                    {
                        activity?.SetTag("risk.reason", riskResult.Reason);
                        activity?.SetStatus(ActivityStatusCode.Error, riskResult.Reason);

                        _logger.LogWarning(
                            "Order {OrderId} rejected by risk analysis. Reason {Reason}",
                            order.Id,
                            riskResult.Reason);

                        var previousStatus = order.Status.ToString();

                        order.MarkAsFailed(riskResult.Reason);

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        await _orderCacheService.RemoveAsync(order.Id, cancellationToken);

                        await PublishStatusChangedAsync(
                            order,
                            previousStatus,
                            riskResult.Reason,
                            cancellationToken);

                        Metrics.OrdersFailed.Add(1);

                        return;
                    }

                    try
                    {
                        var previousStatus = order.Status.ToString();

                        order.MarkAsCompleted(order.Amount);

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        await _orderCacheService.RemoveAsync(order.Id, cancellationToken);

                        await PublishStatusChangedAsync(
                            order,
                            previousStatus,
                            reason: null,
                            cancellationToken);

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

                        await _orderEventPublisher.PublishOrderCompletedAsync(
                            integrationEvent,
                            cancellationToken);

                        _logger.LogInformation(
                            "{Event} - Evento OrderCompleted publicado no Kafka",
                            LogEvents.KafkaEventPublished);

                        Metrics.OrdersProcessed.Add(1);

                        activity?.SetTag("order.status.final", order.Status.ToString());
                        activity?.SetStatus(ActivityStatusCode.Ok);
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        Metrics.OrdersFailed.Add(1);

                        activity?.SetStatus(
                            ActivityStatusCode.Error,
                            "Concurrency conflict while completing order");

                        activity?.AddException(ex);

                        _logger.LogWarning(
                            ex,
                            "Conflito de concorrência ao concluir ordem {OrderId}",
                            order.Id);

                        return;
                    }
                }
            }
            finally
            {
                stopwatch.Stop();

                Metrics.OrderProcessingTime.Record(stopwatch.Elapsed.TotalMilliseconds);

                activity?.SetTag(
                    "processing.duration.ms",
                    stopwatch.Elapsed.TotalMilliseconds);
            }
        }

        private async Task PublishStatusChangedAsync(
            Order order,
            string previousStatus,
            string? reason,
            CancellationToken cancellationToken)
        {
            await _orderEventPublisher.PublishOrderStatusChangedAsync(
                new OrderStatusChangedIntegrationEvent
                {
                    OrderId = order.Id,
                    UserId = order.UserId,
                    Amount = order.Amount,
                    PreviousStatus = previousStatus,
                    NewStatus = order.Status.ToString(),
                    Reason = reason,
                    OccurredAt = DateTime.UtcNow,
                    CorrelationId = _correlationContext.CorrelationId
                },
                cancellationToken);
        }
    }
}