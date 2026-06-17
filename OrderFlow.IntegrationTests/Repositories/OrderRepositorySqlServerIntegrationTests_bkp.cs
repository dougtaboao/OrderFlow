using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Repositories;

namespace OrderFlow.IntegrationTests.Repositories
{
    public class OrderRepositorySqlServerIntegrationTests_bkp
    {
        private const string ConnectionString =
            "Server=localhost,1433;Database=OrderFlowIntegrationTestsDb;User Id=sa;Password=OrderFlow@123;TrustServerCertificate=True";

        public async Task Should_Save_Order_And_Get_By_Id_Using_SqlServer()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            await using var context = new OrderFlowDbContext(options);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var repository = new OrderRepository(context);

            var order = new Order(
                Guid.NewGuid(),
                100m,
                OrderType.Buy,
                OrderPriority.Normal,
                $"EXT-INT-{Guid.NewGuid()}",
                "PETR4",
                10,
                10m,
                null,
                null);

            // Act
            await repository.AddAsync(order);
            await context.SaveChangesAsync();

            var result = await repository.GetByIdAsync(order.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result!.Id);
            Assert.Equal(order.UserId, result.UserId);
            Assert.Equal(order.Amount, result.Amount);
            Assert.Equal(OrderStatus.Created, result.Status);
            Assert.Equal(order.ExternalReference, result.ExternalReference);
        }
    }
}