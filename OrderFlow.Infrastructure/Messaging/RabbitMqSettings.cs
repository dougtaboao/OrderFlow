namespace OrderFlow.Infrastructure.Messaging
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;

        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";

        public string QueueName { get; set; } = "order-created";
        public string DeadLetterQueueName { get; set; } = "order-created-dlq";
        public int MaxRetryCount { get; set; } = 3;
    }
}