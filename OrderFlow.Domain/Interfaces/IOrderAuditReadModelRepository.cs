using OrderFlow.Domain.ReadModels;

namespace OrderFlow.Domain.Interfaces
{
    public interface IOrderAuditReadModelRepository
    {
        Task AddAsync(
            OrderAuditReadModel readModel,
            CancellationToken cancellationToken = default);

        Task<List<OrderAuditReadModel>> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);
    }
}