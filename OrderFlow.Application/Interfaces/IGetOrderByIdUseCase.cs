using OrderFlow.Application.Dtos;

namespace OrderFlow.Application.Interfaces
{
    public interface IGetOrderByIdUseCase
    {
        Task<GetOrderByIdResponse?> ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default);
    }
}