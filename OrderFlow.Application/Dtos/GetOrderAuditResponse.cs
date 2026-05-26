namespace OrderFlow.Application.Dtos
{
    public class GetOrderAuditResponse
    {
        public Guid OrderId { get; set; }

        public Guid UserId { get; set; }

        public decimal Amount { get; set; }

        public string EventType { get; set; } = string.Empty;

        public string CorrelationId { get; set; } = string.Empty;

        public DateTime OccurredAt { get; set; }
    }
}