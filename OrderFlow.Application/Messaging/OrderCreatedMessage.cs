namespace OrderFlow.Application.Messaging
{
    public class OrderCreatedMessage
    {
        public Guid OrderId { get; set; }
    }
}