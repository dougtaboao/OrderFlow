using OrderFlow.Application.Dtos;

namespace OrderFlow.Application.Interfaces
{
    public interface IGetOrderAuditUseCase
    {
        Task<List<GetOrderAuditResponse>> ExecuteAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);
    }
}