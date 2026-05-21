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
    }
}