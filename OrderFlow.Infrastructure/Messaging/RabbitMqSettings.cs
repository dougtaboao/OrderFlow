namespace OrderFlow.Infrastructure.Messaging
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; } = "localhost";
        public string QueueName { get; set; } = "order-created";
        public string DeadLetterQueueName { get; set; } = "order-created-dlq";
        public int MaxRetryCount { get; set; } = 3;
    }
}