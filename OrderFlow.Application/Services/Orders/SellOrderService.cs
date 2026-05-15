using OrderFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderFlow.Application.Services.Orders
{
    public class SellOrderService : ISellOrderService
    {
        public Task ValidateAsync(Order order, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(order.AssetCode))
                throw new InvalidOperationException("Ordem de venda precisa informar o ativo.");

            if (order.Quantity is null or <= 0)
                throw new InvalidOperationException("Ordem de venda precisa informar quantidade maior que zero.");

            if (order.UnitPrice is null or <= 0)
                throw new InvalidOperationException("Ordem de venda precisa informar preço unitário maior que zero.");

            return Task.CompletedTask;
        }

        public Task ExecuteAsync(Order order, CancellationToken cancellationToken = default)
        {
            // Simula integração com sistema de compra
            return Task.Delay(500, cancellationToken);
        }
    }
}
