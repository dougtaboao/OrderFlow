using Microsoft.Extensions.Logging;
using OrderFlow.Application.Dtos;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Messaging;
using OrderFlow.Application.Observability;
using OrderFlow.Application.Security;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Diagnostics;

namespace OrderFlow.Application.UseCases
{
    public class CreateOrderUseCase : ICreateOrderUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOutboxMessageRepository _outboxMessageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICorrelationContext _correlationContext;
        private readonly ILogger<CreateOrderUseCase> _logger;
        private readonly ICreateOrderValidator _validator;
        private readonly ICurrentUser _currentUser;

        public CreateOrderUseCase(
            IOrderRepository orderRepository,
            IOutboxMessageRepository outboxMessageRepository,
            IUnitOfWork unitOfWork,
            ICorrelationContext correlationContext,
            ILogger<CreateOrderUseCase> logger,
            ICreateOrderValidator validator,
            ICurrentUser currentUser)
        {
            _orderRepository = orderRepository;
            _outboxMessageRepository = outboxMessageRepository;
            _unitOfWork = unitOfWork;
            _correlationContext = correlationContext;
            _logger = logger;
            _validator = validator; ;
            _currentUser = currentUser;
        }

        public async Task<CreateOrderResponse> ExecuteAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.UserId == Guid.Empty)
                throw new UnauthorizedAccessException("Usuário autenticado inválido.");

            _validator.Validate(request);

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                [LogProperties.CorrelationId] = _correlationContext.CorrelationId,
                [LogProperties.UserId] = _currentUser.UserId,
                [LogProperties.OrderType] = request.Type,
                [LogProperties.ExternalReference] = request.ExternalReference
            }))
            {
                _logger.LogInformation("{Event} - Iniciando criação da ordem", LogEvents.OrderCreationStarted);

                using var activity = Telemetry.ActivitySource.StartActivity("CreateOrder");

                activity?.SetTag("order.type", request.Type.ToString());
                activity?.SetTag("order.userId", _currentUser.UserId);
                activity?.SetTag("order.externalReference", request.ExternalReference);
                activity?.SetTag("order.amount", request.Amount);
                activity?.SetTag("order.priority", request.Priority.ToString());
                activity?.SetTag("order.assetCode", request.AssetCode);
                activity?.SetTag("order.quantity", request.Quantity);
                activity?.SetTag("order.unitPrice", request.UnitPrice);
                activity?.SetTag("correlation.id", _correlationContext.CorrelationId);

                try
                {
                    var order = new Order(
                    _currentUser.UserId,
                    request.Amount,
                    request.Type,
                    request.Priority,
                    request.ExternalReference,
                    request.AssetCode,
                    request.Quantity,
                    request.UnitPrice,
                    request.SourceAccount,
                    request.DestinationAccount);

                    var integrationMessage = new OrderCreatedMessage
                    {
                        OrderId = order.Id
                    };

                    _logger.LogDebug(
                        "CorrelationId no CreateOrderUseCase: {CorrelationId}",
                        _correlationContext.CorrelationId);

                    var outboxMessage = new OutboxMessage(
                        type: nameof(OrderCreatedMessage),
                        payload: JsonSerializer.Serialize(integrationMessage),
                        correlationId: _correlationContext.CorrelationId);

                    await _orderRepository.AddAsync(order, cancellationToken);
                    await _outboxMessageRepository.AddAsync(outboxMessage, cancellationToken);

                    try
                    {
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        var message = ex.InnerException?.Message ?? ex.Message;
                        throw new Exception($"Erro ao salvar Order + Outbox: {message}", ex);
                    }

                    Metrics.OrdersCreated.Add(1);

                    _logger.LogInformation(
                        "{Event} - Ordem {OrderId} criada e outbox {OutboxMessageId} registrada",
                        LogEvents.OrderCreated,
                        order.Id,
                        outboxMessage.Id);

                    activity?.SetTag("order.id", order.Id);
                    activity?.SetTag("outbox.id", outboxMessage.Id);
                    activity?.SetStatus(ActivityStatusCode.Ok);

                    return new CreateOrderResponse
                    {
                        OrderId = order.Id,
                        Status = order.Status.ToString(),
                        CreatedAt = order.CreatedAt
                    };
                }
                catch (Exception ex)
                {
                    Metrics.OrdersFailed.Add(1);

                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    activity?.AddException(ex);

                    _logger.LogError(
                        ex,
                        "{Event} - Erro ao criar ordem",
                        LogEvents.OrderCreationFailed);

                    throw;
                }


            }
        }
    }
}