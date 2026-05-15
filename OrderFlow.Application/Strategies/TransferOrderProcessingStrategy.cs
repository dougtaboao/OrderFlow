using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Strategies
{
    public class TransferOrderProcessingStrategy : IOrderProcessingStrategy
    {
        public OrderType Type => OrderType.Transfer;

        public Task ProcessAsync(Order order, CancellationToken cancellationToken = default)
        {
            if (order.Amount > 5000)
                throw new Exception("Transferência acima do limite permitido para teste.");

            return Task.CompletedTask;
        }
    }
}