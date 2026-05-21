using OrderFlow.Application.Dtos;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Interfaces
{
    public interface IRiskAnalysisGateway
    {
        Task<RiskAnalysisResult> AnalyzeAsync(
            Order order,
            CancellationToken cancellationToken = default);
    }
}