using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Domain.ReadModels;
using OrderFlow.Infrastructure.Data;

namespace OrderFlow.Infrastructure.Repositories
{
    public class OrderAuditReadModelRepository : IOrderAuditReadModelRepository
    {
        private readonly OrderFlowDbContext _context;

        public OrderAuditReadModelRepository(OrderFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            OrderAuditReadModel readModel,
            CancellationToken cancellationToken = default)
        {
            await _context.OrderAuditReadModels.AddAsync(readModel, cancellationToken);
        }

        public async Task<List<OrderAuditReadModel>> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return await _context.OrderAuditReadModels
                .AsNoTracking()
                .Where(x => x.OrderId == orderId)
                .OrderBy(x => x.OccurredAt)
                .ToListAsync(cancellationToken);
        }
    }
}