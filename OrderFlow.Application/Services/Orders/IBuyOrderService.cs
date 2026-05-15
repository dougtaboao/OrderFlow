using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Services.Orders
{
    public interface IBuyOrderService
    {
        Task ValidateAsync(Order order, CancellationToken cancellationToken = default);
        Task ExecuteAsync(Order order, CancellationToken cancellationToken = default);
    }
}