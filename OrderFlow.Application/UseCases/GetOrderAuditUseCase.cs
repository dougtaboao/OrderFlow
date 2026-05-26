using OrderFlow.Application.Dtos;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.UseCases
{
    public class GetOrderAuditUseCase : IGetOrderAuditUseCase
    {
        private readonly IOrderAuditReadModelRepository _repository;

        public GetOrderAuditUseCase(IOrderAuditReadModelRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetOrderAuditResponse>> ExecuteAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var audits = await _repository.GetByOrderIdAsync(
                orderId,
                cancellationToken);

            return audits
                .Select(x => new GetOrderAuditResponse
                {
                    OrderId = x.OrderId,
                    UserId = x.UserId,
                    Amount = x.Amount,
                    EventType = x.EventType,
                    CorrelationId = x.CorrelationId,
                    OccurredAt = x.OccurredAt
                })
                .ToList();
        }
    }
}