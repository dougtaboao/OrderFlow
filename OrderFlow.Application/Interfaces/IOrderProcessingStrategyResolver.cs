using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Interfaces
{
    public interface IOrderProcessingStrategyResolver
    {
        IOrderProcessingStrategy Resolve(OrderType type);
    }
}