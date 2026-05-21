using OrderFlow.Application.Dtos;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.UseCases
{
    public class GetOrderByIdUseCase : IGetOrderByIdUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderCacheService _orderCacheService;

        public GetOrderByIdUseCase(
            IOrderRepository orderRepository,
            IOrderCacheService orderCacheService)
        {
            _orderRepository = orderRepository;
            _orderCacheService = orderCacheService;
        }

        public async Task<GetOrderByIdResponse?> ExecuteAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var cachedOrder = await _orderCacheService.GetAsync(orderId, cancellationToken);

            if (cachedOrder is not null)
                return cachedOrder;

            var order = await _orderRepository.GetByIdAsNoTrackingAsync(orderId, cancellationToken);

            if (order is null)
                return null;

            var response = MapToResponse(order);

            await _orderCacheService.SetAsync(response, cancellationToken);

            return response;
        }

        public async Task<GetOrderByIdResponse?> ExecuteFreshAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsNoTrackingAsync(orderId, cancellationToken);

            if (order is null)
                return null;

            return MapToResponse(order);
        }

        private static GetOrderByIdResponse MapToResponse(Order order)
        {
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