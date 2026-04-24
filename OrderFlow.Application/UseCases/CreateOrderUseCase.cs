using System.Text.Json;
using Microsoft.Extensions.Logging;
using OrderFlow.Application.Dtos;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Messaging;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.UseCases
{
    public class CreateOrderUseCase : ICreateOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOutboxMessageRepository _outboxMessageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICorrelationContext _correlationContext;
        private readonly ILogger<CreateOrderUseCase> _logger;

        public CreateOrderUseCase(
            IOrderRepository orderRepository,
            IOutboxMessageRepository outboxMessageRepository,
            IUnitOfWork unitOfWork,
            ICorrelationContext correlationContext,
            ILogger<CreateOrderUseCase> logger)
        {
            _orderRepository = orderRepository;
            _outboxMessageRepository = outboxMessageRepository;
            _unitOfWork = unitOfWork;
            _correlationContext = correlationContext;
            _logger = logger;
        }

        public async Task<CreateOrderResponse> ExecuteAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Iniciando criação de ordem para UserId {UserId} com Amount {Amount}",
                request.UserId,
                request.Amount);

            var order = new Order(request.UserId, request.Amount, request.Type, request.Priority, request.ExternalReference);

            var integrationMessage = new OrderCreatedMessage
            {
                OrderId = order.Id
            };

            var outboxMessage = new OutboxMessage(
                type: nameof(OrderCreatedMessage),
                payload: JsonSerializer.Serialize(integrationMessage),
                correlationId: _correlationContext.CorrelationId);

            await _orderRepository.AddAsync(order, cancellationToken);
            await _outboxMessageRepository.AddAsync(outboxMessage, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Ordem {OrderId} criada com sucesso e mensagem registrada na outbox",
                order.Id);

            return new CreateOrderResponse
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt
            };
        }
    }
}