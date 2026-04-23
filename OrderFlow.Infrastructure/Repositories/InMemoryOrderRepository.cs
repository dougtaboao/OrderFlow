using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    public class InMemoryOrderRepository //: IOrderRepository
    {
        private static readonly List<Order> Orders = new();

        public Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            Orders.Add(order);
            return Task.CompletedTask;
        }

        public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var order = Orders.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(order);
        }

        public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
        {
            var existingOrder = Orders.FirstOrDefault(x => x.Id == order.Id);

            if (existingOrder is not null)
            {
                Orders.Remove(existingOrder);
                Orders.Add(order);
            }

            return Task.CompletedTask;
        }
    }
}