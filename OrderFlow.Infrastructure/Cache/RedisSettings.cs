namespace OrderFlow.Infrastructure.Cache
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; } = "localhost:6379";
        public int OrderCacheExpirationMinutes { get; set; } = 5;
    }
}