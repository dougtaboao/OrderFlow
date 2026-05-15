using OrderFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderFlow.Application.Services.Orders
{
    public interface ITransferOrderService
    {
        Task ValidateAsync(Order order, CancellationToken cancellationToken = default);
        Task ExecuteAsync(Order order, CancellationToken cancellationToken = default);
    }
}
