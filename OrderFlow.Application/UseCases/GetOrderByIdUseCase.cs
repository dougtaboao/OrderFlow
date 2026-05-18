using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderFlow.Application.Dtos;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.UseCases
{
    public class GetOrderByIdUseCase : IGetOrderByIdUseCase
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdUseCase(IOrderRepository orderRepository, ILogger<ProcessOrderUseCase> logger)
        {
            _orderRepository = orderRepository;
        }

        public async Task<GetOrderByIdResponse?> ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

            if (order is null)
                return null;

            return new GetOrderByIdResponse
            {
                OrderId = order.Id,
                UserId = order.UserId,
                Amount = order.Amount,
                Status = order.Status.ToString(),
                Type = order.Type.ToString(),
                Priority = order.Priority.ToString(),
                ExternalReference = order.ExternalReference,
                AssetCode = order.AssetCode,
                Quantity = order.Quantity,
                UnitPrice = order.UnitPrice,
                SourceAccount = order.SourceAccount,
                DestinationAccount = order.DestinationAccount,
                CreatedAt = order.CreatedAt,
                Events = order.Events
                    .Select(e => new OrderEventDto
                    {
                        Type = e.Type.ToString(),
                        Description = e.Description,
                        CreatedAt = e.CreatedAt
                    })
                    .ToList()
            };
        }
    }
}