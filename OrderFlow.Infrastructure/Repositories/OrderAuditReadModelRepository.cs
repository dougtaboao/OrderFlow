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
    }
}