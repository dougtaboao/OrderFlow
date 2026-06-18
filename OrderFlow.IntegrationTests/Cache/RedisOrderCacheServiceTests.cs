using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using OrderFlow.Application.Dtos;
using OrderFlow.Infrastructure.Cache;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace OrderFlow.IntegrationTests.Cache
{
    public class RedisOrderCacheServiceTests : IAsyncLifetime
    {
        private readonly RedisContainer _redisContainer = new RedisBuilder()
            .WithImage("redis:7")
            .Build();

        private IConnectionMultiplexer _connection = null!;
        private RedisOrderCacheService _cacheService = null!;

        public async Task InitializeAsync()
        {
            await _redisContainer.StartAsync();

            var connectionString = _redisContainer.GetConnectionString();

            _connection = await ConnectionMultiplexer.ConnectAsync(connectionString);

            var settings = new RedisSettings
            {
                ConnectionString = connectionString,
                OrderCacheExpirationMinutes = 5
            };

            _cacheService = new RedisOrderCacheService(
                _connection,
                settings,
                NullLogger<RedisOrderCacheService>.Instance);
        }

        public async Task DisposeAsync()
        {
            await _connection.CloseAsync();
            _connection.Dispose();

            await _redisContainer.DisposeAsync();
        }

        [Fact]
        public async Task SetAsync_Should_Save_Order_In_Redis()
        {
            var order = CreateOrderResponse();

            await _cacheService.SetAsync(order);

            var result = await _cacheService.GetAsync(order.OrderId);

            result.Should().NotBeNull();
            result!.OrderId.Should().Be(order.OrderId);
        }

        [Fact]
        public async Task GetAsync_Should_Return_Null_When_Order_Is_Not_In_Cache()
        {
            var result = await _cacheService.GetAsync(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Fact]
        public async Task RemoveAsync_Should_Delete_Order_From_Redis()
        {
            var order = CreateOrderResponse();

            await _cacheService.SetAsync(order);
            await _cacheService.RemoveAsync(order.OrderId);

            var result = await _cacheService.GetAsync(order.OrderId);

            result.Should().BeNull();
        }

        private static GetOrderByIdResponse CreateOrderResponse()
        {
            return new GetOrderByIdResponse
            {
                OrderId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Quantity = 10,
                Amount = 150.75m,
                Status = "Created",
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}