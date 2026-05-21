using Microsoft.Extensions.Logging;
using OrderFlow.Application.Dtos;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Observability;
using StackExchange.Redis;
using System.Text.Json;

namespace OrderFlow.Infrastructure.Cache
{
    public class RedisOrderCacheService : IOrderCacheService
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly RedisSettings _settings;
        private readonly ILogger<RedisOrderCacheService> _logger;

        public RedisOrderCacheService(
            IConnectionMultiplexer connection,
            RedisSettings settings,
            ILogger<RedisOrderCacheService> logger)
        {
            _connection = connection;
            _settings = settings;
            _logger = logger;
        }

        public async Task<GetOrderByIdResponse?> GetAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var database = _connection.GetDatabase();

            var key = GetKey(orderId);
            var value = await database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
            {
                _logger.LogInformation(
                    "{Event} - Cache miss para OrderId {OrderId}",
                    LogEvents.OrderCacheMiss,
                    orderId);

                Metrics.OrderCacheMisses.Add(1);

                return null;
            }

            _logger.LogInformation(
                "{Event} - Cache hit para OrderId {OrderId}",
                LogEvents.OrderCacheHit,
                orderId);

            Metrics.OrderCacheHits.Add(1);

            return JsonSerializer.Deserialize<GetOrderByIdResponse>(value.ToString());
        }

        public async Task SetAsync(
            GetOrderByIdResponse order,
            CancellationToken cancellationToken = default)
        {
            var database = _connection.GetDatabase();

            var key = GetKey(order.OrderId);
            var json = JsonSerializer.Serialize(order);

            await database.StringSetAsync(
                key,
                json,
                TimeSpan.FromMinutes(_settings.OrderCacheExpirationMinutes));

            _logger.LogInformation(
                "{Event} - Cache gravado para OrderId {OrderId} com TTL {TTLMinutes} minutos",
                LogEvents.OrderCacheSet,
                order.OrderId,
                _settings.OrderCacheExpirationMinutes);

            Metrics.OrderCacheSets.Add(1);
        }

        public async Task RemoveAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var database = _connection.GetDatabase();

            var key = GetKey(orderId);

            await database.KeyDeleteAsync(key);

            _logger.LogInformation(
                "{Event} - Cache removido para OrderId {OrderId}",
                LogEvents.OrderCacheRemoved,
                orderId);

            Metrics.OrderCacheRemovals.Add(1);
        }

        private static string GetKey(Guid orderId)
        {
            return $"order:{orderId}";
        }
    }
}