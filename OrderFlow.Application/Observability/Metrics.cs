using System.Diagnostics.Metrics;

namespace OrderFlow.Application.Observability
{
    public static class Metrics
    {
        private static readonly Meter Meter = new("OrderFlow");

        public static readonly Counter<int> OrdersCreated =
            Meter.CreateCounter<int>("orders_created");

        public static readonly Counter<int> OrdersProcessed =
            Meter.CreateCounter<int>("orders_processed");

        public static readonly Counter<int> OrdersFailed =
            Meter.CreateCounter<int>("orders_failed");

        public static readonly Histogram<double> OrderProcessingTime =
            Meter.CreateHistogram<double>("order_processing_time");

        public static readonly Counter<int> OrderCacheHits =
            Meter.CreateCounter<int>("order_cache_hits");

        public static readonly Counter<int> OrderCacheMisses =
            Meter.CreateCounter<int>("order_cache_misses");

        public static readonly Counter<int> OrderCacheSets =
            Meter.CreateCounter<int>("order_cache_sets");

        public static readonly Counter<int> OrderCacheRemovals =
            Meter.CreateCounter<int>("order_cache_removals");

        public static readonly Counter<int> RabbitMessagesReceived =
            Meter.CreateCounter<int>("rabbit_messages_received");

        public static readonly Counter<int> RabbitMessagesRetried =
            Meter.CreateCounter<int>("rabbit_messages_retried");

        public static readonly Counter<int> RabbitMessagesDeadLettered =
            Meter.CreateCounter<int>("rabbit_messages_dead_lettered");

        public static readonly Counter<int> KafkaAuditEventsConsumed =
            Meter.CreateCounter<int>("kafka_audit_events_consumed");

        public static readonly Counter<int> KafkaConsumerErrors =
            Meter.CreateCounter<int>("kafka_consumer_errors");
    }
}
