namespace OrderFlow.Application.Messaging
{
    public class OrderCompletedIntegrationEvent
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CompletedAt { get; set; }
        public string CorrelationId { get; set; } = string.Empty;
    }
}