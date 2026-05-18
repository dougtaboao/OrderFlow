using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Infrastructure.Data;

namespace OrderFlow.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderFlowDbContext _context;

        public OrderRepository(OrderFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _context.Orders.AddAsync(order, cancellationToken);
            // await _context.SaveChangesAsync(cancellationToken); // vai salvar no Iunit work
        }

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (_context is null)
                throw new Exception("_context null");

            if (_context.Orders is null)
                throw new Exception("_context.Orders null");

            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);

            if (!canConnect)
                throw new Exception("Banco não conectou");

            return await _context.Orders
                .Include(o => o.Events)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task Update(Order order, CancellationToken cancellationToken = default)
        {
            _context.Orders.Update(order);
            // await _context.SaveChangesAsync(cancellationToken); // vai salvar no Iunit work
        }
    }
}