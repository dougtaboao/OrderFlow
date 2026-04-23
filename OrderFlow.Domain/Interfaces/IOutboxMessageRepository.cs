using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces
{
    public interface IOutboxMessageRepository
    {
        Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);
        Task<List<OutboxMessage>> GetPendingMessagesAsync(int take, CancellationToken cancellationToken = default);
        Task Update(OutboxMessage message);
    }
}