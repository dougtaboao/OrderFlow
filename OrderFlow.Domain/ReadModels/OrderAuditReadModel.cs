namespace OrderFlow.Domain.ReadModels
{
    public class OrderAuditReadModel
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public Guid OrderId { get; private set; }

        public Guid UserId { get; private set; }

        public decimal Amount { get; private set; }

        public string EventType { get; private set; } = string.Empty;

        public string CorrelationId { get; private set; } = string.Empty;

        public DateTime OccurredAt { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        private OrderAuditReadModel()
        {
        }

        public OrderAuditReadModel(
            Guid orderId,
            Guid userId,
            decimal amount,
            string eventType,
            string correlationId,
            DateTime occurredAt)
        {
            OrderId = orderId;
            UserId = userId;
            Amount = amount;
            EventType = eventType;
            CorrelationId = correlationId;
            OccurredAt = occurredAt;
        }
    }
}