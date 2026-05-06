namespace OrderFlow.Application.Observability
{
    public static class LogEvents
    {
        public const string OrderCreationStarted = "OrderCreationStarted";
        public const string OrderCreated = "OrderCreated";
        public const string OutboxMessageCreated = "OutboxMessageCreated";
        public const string OutboxPublishingStarted = "OutboxPublishingStarted";
        public const string OutboxMessagePublished = "OutboxMessagePublished";
        public const string RabbitMessageReceived = "RabbitMessageReceived";
        public const string OrderProcessingStarted = "OrderProcessingStarted";
        public const string OrderProcessingIgnored = "OrderProcessingIgnored";
        public const string OrderProcessingFailed = "OrderProcessingFailed";
        public const string OrderCompleted = "OrderCompleted";
        public const string MessageRetried = "MessageRetried";
        public const string MessageSentToDlq = "MessageSentToDlq";
        public const string KafkaEventPublished = "KafkaEventPublished";
    }
}