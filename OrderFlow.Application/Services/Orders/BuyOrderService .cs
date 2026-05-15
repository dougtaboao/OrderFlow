using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Services.Orders
{
    public class BuyOrderService : IBuyOrderService
    {
        public Task ValidateAsync(Order order, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(order.AssetCode))
                throw new InvalidOperationException("Ordem de compra precisa informar o ativo.");

            if (order.Quantity is null or <= 0)
                throw new InvalidOperationException("Ordem de compra precisa informar quantidade maior que zero.");

            if (order.UnitPrice is null or <= 0)
                throw new InvalidOperationException("Ordem de compra precisa informar preço unitário maior que zero.");

            if (order.Amount != order.Quantity.Value * order.UnitPrice.Value)
                throw new InvalidOperationException("Valor total da compra não confere com quantidade x preço unitário.");

            return Task.CompletedTask;
        }

        public Task ExecuteAsync(Order order, CancellationToken cancellationToken = default)
        {
            // Simula integração com sistema de compra
            return Task.Delay(500, cancellationToken);
        }
    }
}