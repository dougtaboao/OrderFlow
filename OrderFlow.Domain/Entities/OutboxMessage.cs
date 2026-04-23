using OrderFlow.Domain.Common;

namespace OrderFlow.Domain.Entities
{
    public class OutboxMessage : BaseEntity
    {
        public string Type { get; private set; } = string.Empty;
        public string Payload { get; private set; } = string.Empty;
        public string CorrelationId { get; private set; } = string.Empty;
        public DateTime? ProcessedAt { get; private set; }
        public string? Error { get; private set; }

        private OutboxMessage()
        {
        }

        public OutboxMessage(string type, string payload, string correlationId)
            : base(Guid.NewGuid())
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("O tipo da mensagem deve ser informado.");

            if (string.IsNullOrWhiteSpace(payload))
                throw new ArgumentException("O payload da mensagem deve ser informado.");

            if (string.IsNullOrWhiteSpace(correlationId))
                throw new ArgumentException("O correlationId deve ser informado.");

            Type = type;
            Payload = payload;
        }

        public void MarkAsProcessed()
        {
            ProcessedAt = DateTime.UtcNow;
            Error = null;
        }

        public void MarkAsFailed(string error)
        {
            Error = error;
        }
    }
}