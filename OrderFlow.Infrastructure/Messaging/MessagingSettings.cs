namespace OrderFlow.Infrastructure.Messaging
{
    public class MessagingSettings
    {
        public MessagingProvider Provider { get; set; } = MessagingProvider.RabbitMq;
    }
}