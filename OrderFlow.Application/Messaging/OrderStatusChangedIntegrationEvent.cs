namespace OrderFlow.Application.Messaging
{
    public class OrderStatusChangedIntegrationEvent
    {
        public Guid OrderId { get; set; }

        public Guid UserId { get; set; }

        public decimal Amount { get; set; }

        public string PreviousStatus { get; set; } = string.Empty;

        public string NewStatus { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public DateTime OccurredAt { get; set; }

        public string CorrelationId { get; set; } = string.Empty;
    }
}