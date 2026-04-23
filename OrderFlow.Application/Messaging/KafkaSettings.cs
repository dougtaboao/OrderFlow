namespace OrderFlow.Infrastructure.Messaging
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = "localhost:9092";
        public string OrderCompletedTopic { get; set; } = "order-completed";
    }
}