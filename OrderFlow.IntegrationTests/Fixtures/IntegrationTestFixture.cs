using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using System.ComponentModel;
using Testcontainers.Kafka;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace OrderFlow.IntegrationTests.Fixtures
{
    public sealed class IntegrationTestFixture : IAsyncLifetime
    {
        private const string UserName = "guest";
        private const string Password = "guest";

        private readonly MsSqlContainer _sqlServerContainer =
            new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("OrderFlow@123")
                .Build();

        private readonly RedisContainer _redisContainer =
            new RedisBuilder("redis:latest")
                .Build();

        private readonly RabbitMqContainer _rabbitMqContainer =
            new RabbitMqBuilder("rabbitmq:3-management")
                .WithUsername(UserName)
                .WithPassword(Password)
                .Build();

        private readonly KafkaContainer _kafkaContainer =
            new KafkaBuilder("confluentinc/cp-kafka:7.6.1")
                .Build();

        public string SqlServerConnectionString =>
            _sqlServerContainer.GetConnectionString();

        public string RedisConnectionString =>
            $"{_redisContainer.Hostname}:{_redisContainer.GetMappedPublicPort(6379)}";

        public string RabbitMqHost =>
            _rabbitMqContainer.Hostname;

        public ushort RabbitMqPort =>
            (ushort)_rabbitMqContainer.GetMappedPublicPort(5672);

        public string KafkaBootstrapServers =>
            _kafkaContainer.GetBootstrapAddress();

        public async Task InitializeAsync()
        {
            await _sqlServerContainer.StartAsync();
            await _redisContainer.StartAsync();
            await _rabbitMqContainer.StartAsync();
            await _kafkaContainer.StartAsync();

            await using var context = CreateContext();

            await context.Database.MigrateAsync();
        }

        public OrderFlowDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
                .UseSqlServer(SqlServerConnectionString)
                .Options;

            return new OrderFlowDbContext(options);
        }

        public async Task ResetAsync()
        {
            await using var context = CreateContext();

            context.OrderEvents.RemoveRange(context.OrderEvents);
            context.OutboxMessages.RemoveRange(context.OutboxMessages);
            context.Orders.RemoveRange(context.Orders);

            await context.SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            await _kafkaContainer.DisposeAsync();
            await _rabbitMqContainer.DisposeAsync();
            await _redisContainer.DisposeAsync();
            await _sqlServerContainer.DisposeAsync();
        }
    }
}