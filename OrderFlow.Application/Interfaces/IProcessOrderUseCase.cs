using OrderFlow.Application.Dtos;

namespace OrderFlow.Application.Interfaces
{
    public interface IProcessOrderUseCase
    {
        Task ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default);
    }
}