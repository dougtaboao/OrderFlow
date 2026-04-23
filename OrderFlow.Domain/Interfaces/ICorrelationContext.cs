namespace OrderFlow.Application.Interfaces
{
    public interface ICorrelationContext
    {
        string CorrelationId { get; }
        void Set(string correlationId);
    }
}