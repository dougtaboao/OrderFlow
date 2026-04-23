using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Infrastructure.Data;

namespace OrderFlow.Infrastructure.Repositories
{
    public class OutboxMessageRepository : IOutboxMessageRepository
    {
        private readonly OrderFlowDbContext _context;

        public OutboxMessageRepository(OrderFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            await _context.OutboxMessages.AddAsync(message, cancellationToken);
            // await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<OutboxMessage>> GetPendingMessagesAsync(int take, CancellationToken cancellationToken = default)
        {
            return await _context.OutboxMessages
                .Where(x => x.ProcessedAt == null)
                .OrderBy(x => x.CreatedAt)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task Update(OutboxMessage message)
        {
            _context.OutboxMessages.Update(message);
            // await _context.SaveChangesAsync(cancellationToken);
        }
    }
}