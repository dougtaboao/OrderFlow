using OrderFlow.Domain.Common;
using OrderFlow.Domain.Enums;

namespace OrderFlow.Domain.Entities
{
    public class OrderEvent : BaseEntity
    {
        // public Guid Id { get; private set; } criado na Classe Base após refatoração
        public Guid OrderId { get; private set; }
        public OrderEventType Type { get; private set; }
        public string Description { get; private set; }
        // public DateTime CreatedAt { get; private set; } criado na Classe Base após refatoração

        public OrderEvent()
        {
                
        }

        public OrderEvent(Guid orderId, OrderEventType type, string description)
            : base(Guid.NewGuid())
        {
            if (orderId == Guid.Empty)
                throw new ArgumentException("OrderId inválido.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("A descrição do evento deve ser informada.");

            // Id = Guid.NewGuid(); criado na Classe Base após refatoração
            OrderId = orderId;
            Type = type;
            Description = description;
            // CreatedAt = DateTime.UtcNow; criado na Classe Base após refatoração
        }
    }
}