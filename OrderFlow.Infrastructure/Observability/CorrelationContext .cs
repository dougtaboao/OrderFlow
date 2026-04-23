using OrderFlow.Application.Interfaces;

namespace OrderFlow.Infrastructure.Observability
{
    public class CorrelationContext : ICorrelationContext
    {
        public string CorrelationId { get; private set; } = string.Empty;

        public void Set(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}