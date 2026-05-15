using OrderFlow.Domain.Common;
using OrderFlow.Domain.Enums;

namespace OrderFlow.Domain.Entities
{
    public class Order : BaseEntity
    {
        private readonly List<OrderEvent> _events = new();

        // public Guid Id { get; private set; } criado na Classe Base após refatoração
        // public DateTime CreatedAt { get; private set; } criado na Classe Base após refatoração

        public Guid UserId { get; private set; }
        public decimal Amount { get; private set; }
        public OrderStatus Status { get; private set; }
        public OrderType Type { get; private set; }
        public OrderPriority Priority { get; private set; }
        public string ExternalReference { get; private set; } = string.Empty;
        public string? AssetCode { get; private set; }
        public int? Quantity { get; private set; }
        public decimal? UnitPrice { get; private set; }
        public string? SourceAccount { get; private set; }
        public string? DestinationAccount { get; private set; }

        public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

        public IReadOnlyCollection<OrderEvent> Events => _events.AsReadOnly();

        private Order()
        {
        }

        public Order(Guid userId, decimal amount, OrderType type, OrderPriority priority, string externalReference,
                        string? assetCode, int? quantity, decimal? unitPrice, string? sourceAccount, string? destinationAccount) : base(Guid.NewGuid())
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId inválido.");

            if (amount <= 0)
                throw new ArgumentException("O valor da ordem deve ser maior que zero.");

            if (string.IsNullOrWhiteSpace(externalReference))
                throw new ArgumentException("ExternalReference deve ser informado.");

            // Id = Guid.NewGuid(); criado na Classe Base após refatoração
            UserId = userId;
            Amount = amount;

            Type = type;
            Priority = priority;
            ExternalReference = externalReference;

            AssetCode = assetCode;
            Quantity = quantity;
            UnitPrice = unitPrice;
            SourceAccount = sourceAccount;
            DestinationAccount = destinationAccount;

            Status = OrderStatus.Created;
            // CreatedAt = DateTime.UtcNow; criado na Classe Base após refatoração

            AddEvent(OrderEventType.Created, $"Ordem {amount} criada com valor.");
        }

        public bool CanBeProcessed()
        {
            return Status == OrderStatus.Created;
        }

        public void MarkAsProcessing(decimal amount)
        {
            if (!CanBeProcessed())
                throw new InvalidOperationException("Somente ordens criadas podem ir para processamento.");

            Status = OrderStatus.Processing;
            AddEvent(OrderEventType.Processing, $"Ordem {amount} em processamento.");
        }

        public void MarkAsCompleted(decimal amount)
        {
            if (Status != OrderStatus.Processing)
                throw new InvalidOperationException("Somente ordens em processamento podem ser concluídas.");

            Status = OrderStatus.Completed;
            AddEvent(OrderEventType.Completed, $"Ordem {amount} processada com sucesso.");
        }

        public void MarkAsFailed(string reason)
        {
            if (Status != OrderStatus.Processing && Status != OrderStatus.Created)
                throw new InvalidOperationException("Somente ordens criadas ou em processamento podem falhar.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("O motivo da falha deve ser informado.");

            Status = OrderStatus.Failed;
            AddEvent(OrderEventType.Failed, $"Falha no processamento: {reason}");
        }

        private void AddEvent(OrderEventType type, string description)
        {
            _events.Add(new OrderEvent(Id, type, description));
        }
    }
}