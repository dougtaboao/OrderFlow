using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Messaging;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.UseCases
{
    public class ProcessOrderUseCase : IProcessOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessOrderUseCase> _logger;
        private readonly IOrderEventPublisher _orderEventPublisher;
        private readonly ICorrelationContext _correlationContext;

        public ProcessOrderUseCase(
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork,
            ILogger<ProcessOrderUseCase> logger,
            IOrderEventPublisher orderEventPublisher,
            ICorrelationContext correlationContext)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _orderEventPublisher = orderEventPublisher;
            _correlationContext = correlationContext;
        }

        public async Task ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Iniciando processamento da ordem {OrderId}", orderId);

            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

            if (order is null)
            {
                _logger.LogWarning("Ordem {OrderId} não encontrada para processamento", orderId);
                throw new InvalidOperationException($"Ordem {orderId} não encontrada.");
            }

            if (!order.CanBeProcessed())
            {
                _logger.LogWarning(
                    "Ordem {OrderId} ignorada por idempotência. Status atual {Status}",
                    order.Id,
                    order.Status);

                return;
            }

            try
            {
                order.MarkAsProcessing(order.Amount);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Ordem {OrderId} movida para Processing", order.Id);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogWarning(
                    "Conflito de concorrência ao mover ordem {OrderId} para Processing",
                    order.Id);

                return;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Transição inválida ao mover ordem {OrderId} para Processing",
                    order.Id);

                return;
            }

            await Task.Delay(2000, cancellationToken);

            if (order.Amount > 1000)
            {
                _logger.LogError( 
                    "Falha simulada para ordem {OrderId} com Amount {Amount}",
                    order.Id,
                    order.Amount);

                order.MarkAsFailed("Falha simulada para testes de retry e DLQ.");
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                throw new Exception("Falha simulada para testes de retry e DLQ.");

            }

            try
            {
                order.MarkAsCompleted(order.Amount);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var integrationEvent = new OrderCompletedIntegrationEvent
                {
                    OrderId = order.Id,
                    UserId = order.UserId,
                    Amount = order.Amount,
                    CompletedAt = DateTime.UtcNow,
                    CorrelationId = _correlationContext.CorrelationId
                };

                await _orderEventPublisher.PublishOrderCompletedAsync(integrationEvent, cancellationToken);

                _logger.LogInformation(
                    "Ordem {OrderId} concluída com sucesso e evento publicado no Kafka",
                    order.Id);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogWarning(
                    "Conflito de concorrência ao concluir ordem {OrderId}",
                    order.Id);
            }
        }
    }
}