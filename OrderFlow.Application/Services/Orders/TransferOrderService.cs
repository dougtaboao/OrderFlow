using OrderFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderFlow.Application.Services.Orders
{
    public class TransferOrderService : ITransferOrderService
    {
        public Task ValidateAsync(Order order, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(order.SourceAccount))
                throw new InvalidOperationException("Transferência precisa informar conta de origem.");

            if (string.IsNullOrWhiteSpace(order.DestinationAccount))
                throw new InvalidOperationException("Transferência precisa informar conta de destino.");

            if (order.SourceAccount == order.DestinationAccount)
                throw new InvalidOperationException("Conta de origem e destino não podem ser iguais.");

            if (order.Amount > 5000)
                throw new InvalidOperationException("Transferência acima do limite permitido.");

            return Task.CompletedTask;
        }

        public Task ExecuteAsync(Order order, CancellationToken cancellationToken = default)
        {
            // Simula integração com sistema de compra
            return Task.Delay(500, cancellationToken);
        }
    }
}
