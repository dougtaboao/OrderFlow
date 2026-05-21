using OrderFlow.Application.Dtos;

namespace OrderFlow.Application.Interfaces
{
    public interface IGetOrderByIdUseCase
    {
        Task<GetOrderByIdResponse?> ExecuteAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        Task<GetOrderByIdResponse?> ExecuteFreshAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);
    }
}