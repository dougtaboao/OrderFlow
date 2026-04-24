using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Dtos
{
    public class CreateOrderRequest
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public OrderType Type { get; set; }
        public OrderPriority Priority { get; set; }

        public string ExternalReference { get; set; } = string.Empty;
    }
}