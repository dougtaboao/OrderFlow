namespace OrderFlow.Application.Dtos
{
    public class GetOrderByIdResponse
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<OrderEventDto> Events { get; set; } = new();
    }
}