namespace OrderFlow.Application.Observability
{
    public static class LogProperties
    {
        public const string CorrelationId = "CorrelationId";
        public const string OrderId = "OrderId";
        public const string UserId = "UserId";
        public const string OrderType = "OrderType";
        public const string Status = "Status";
        public const string ExternalReference = "ExternalReference";
        public const string OutboxMessageId = "OutboxMessageId";
        public const string QueueName = "QueueName";
        public const string RetryCount = "RetryCount";
    }
}