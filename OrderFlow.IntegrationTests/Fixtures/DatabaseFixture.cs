using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using Testcontainers.MsSql;

namespace OrderFlow.IntegrationTests.Fixtures
{
    public sealed class DatabaseFixture : IAsyncLifetime
    {
        private readonly MsSqlContainer _container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("OrderFlow@123")
            .Build();

        public string ConnectionString => _container.GetConnectionString();

        public async Task InitializeAsync()
        {
            await _container.StartAsync();

            await using var context = CreateContext();

            await context.Database.MigrateAsync();
        }

        public OrderFlowDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            return new OrderFlowDbContext(options);
        }

        public async Task ClearDatabaseAsync()
        {
            await using var context = CreateContext();

            context.OrderEvents.RemoveRange(context.OrderEvents);
            context.Orders.RemoveRange(context.Orders);

            await context.SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }
}