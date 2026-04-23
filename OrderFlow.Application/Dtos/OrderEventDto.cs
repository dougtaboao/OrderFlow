namespace OrderFlow.Application.Dtos
{
    public class OrderEventDto
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}