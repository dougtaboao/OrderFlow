using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Strategies
{
    public class SellOrderProcessingStrategy : IOrderProcessingStrategy
    {
        public OrderType Type => OrderType.Sell;

        public Task ProcessAsync(Order order, CancellationToken cancellationToken = default)
        {
            if (order.Amount <= 0)
                throw new Exception("Ordem de venda inválida.");

            return Task.CompletedTask;
        }
    }
}
