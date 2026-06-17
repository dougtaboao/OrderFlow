using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;

namespace OrderFlow.IntegrationTests.Fixtures
{
    public class DatabaseFixture : IAsyncLifetime
    {
        public string ConnectionString =
            "Server=localhost,1433;Database=OrderFlowIntegrationTestsDb;User Id=sa;Password=OrderFlow@123;TrustServerCertificate=True";

        public DbContextOptions<OrderFlowDbContext> Options { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            Options = new DbContextOptionsBuilder<OrderFlowDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            await using var context = new OrderFlowDbContext(Options);

            await context.Database.EnsureDeletedAsync();
            // await context.Database.EnsureCreatedAsync(); // Recria o banco em status atual
            await context.Database.MigrateAsync(); // Assim é otimo para homol e prod pq recria as migrações em ordem validando as alterações pro banco atual 
        }

        public async Task DisposeAsync()
        {
            await using var context = new OrderFlowDbContext(Options);

            await context.Database.EnsureDeletedAsync();
        }

        public async Task ClearDatabaseAsync()
        {
            await using var context = CreateContext();

            await context.OrderAuditReadModels.ExecuteDeleteAsync();
            await context.OutboxMessages.ExecuteDeleteAsync();
            await context.Orders.ExecuteDeleteAsync();

            await context.SaveChangesAsync();
        }

        public OrderFlowDbContext CreateContext()
        {
            return new OrderFlowDbContext(Options);
        }
    }
}