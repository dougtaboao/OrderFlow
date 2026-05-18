namespace OrderFlow.Application.Dtos
{
    public class GetOrderByIdResponse
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }

        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string ExternalReference { get; set; } = string.Empty;

        public string? AssetCode { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }

        public string? SourceAccount { get; set; }
        public string? DestinationAccount { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<OrderEventDto> Events { get; set; } = new();
    }
}