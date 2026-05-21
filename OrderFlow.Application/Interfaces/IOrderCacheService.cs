using OrderFlow.Application.Dtos;

namespace OrderFlow.Application.Interfaces
{
    public interface IOrderCacheService
    {
        Task<GetOrderByIdResponse?> GetAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task SetAsync(GetOrderByIdResponse order, CancellationToken cancellationToken = default);

        Task RemoveAsync(Guid orderId, CancellationToken cancellationToken = default);
    }
}
