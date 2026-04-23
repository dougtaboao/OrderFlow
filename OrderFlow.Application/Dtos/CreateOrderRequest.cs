namespace OrderFlow.Application.Dtos
{
    public class CreateOrderRequest
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public Guid Myguid2 { get; set; }
    }
}