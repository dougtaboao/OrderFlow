using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Interfaces
{
    public interface IOrderProcessingStrategy
    {
        OrderType Type { get; }

        Task ProcessAsync(Order order, CancellationToken cancellationToken = default);
    }
}
