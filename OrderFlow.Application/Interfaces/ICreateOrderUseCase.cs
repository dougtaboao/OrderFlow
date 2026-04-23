using OrderFlow.Application.Dtos;

namespace OrderFlow.Application.Interfaces
{
    public interface ICreateOrderUseCase
    {
        Task<CreateOrderResponse> ExecuteAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    }
}