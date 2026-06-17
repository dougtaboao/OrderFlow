using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using OrderFlow.Application.Dtos;
using OrderFlow.Infrastructure.Cache;
using StackExchange.Redis;

namespace OrderFlow.IntegrationTests.Cache
{
    public class RedisOrderCacheServiceTests : IAsyncLifetime
    {
        private IConnectionMultiplexer _connection = null!;
        private RedisOrderCacheService _cacheService = null!;

        public async Task InitializeAsync()
        {
            _connection = await ConnectionMultiplexer.ConnectAsync(
                "localhost:6379,abortConnect=false");

            var settings = new RedisSettings
            {
                ConnectionString = "localhost:6379,abortConnect=false",
                OrderCacheExpirationMinutes = 5
            };

            _cacheService = new RedisOrderCacheService(
                _connection,
                settings,
                NullLogger<RedisOrderCacheService>.Instance);
        }

        public async Task DisposeAsync()
        {
            var database = _connection.GetDatabase();
            await database.ExecuteAsync("FLUSHDB");

            await _connection.CloseAsync();
            _connection.Dispose();
        }

        [Fact]
        public async Task SetAsync_Should_Save_Order_In_Redis()
        {
            // Arrange
            var order = CreateOrderResponse();

            // Act
            await _cacheService.SetAsync(order);

            var result = await _cacheService.GetAsync(order.OrderId);

            // Assert
            result.Should().NotBeNull();
            result!.OrderId.Should().Be(order.OrderId);
        }

        [Fact]
        public async Task GetAsync_Should_Return_Null_When_Order_Is_Not_In_Cache()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            // Act
            var result = await _cacheService.GetAsync(orderId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task RemoveAsync_Should_Delete_Order_From_Redis()
        {
            // Arrange
            var order = CreateOrderResponse();

            await _cacheService.SetAsync(order);

            // Act
            await _cacheService.RemoveAsync(order.OrderId);

            var result = await _cacheService.GetAsync(order.OrderId);

            // Assert
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