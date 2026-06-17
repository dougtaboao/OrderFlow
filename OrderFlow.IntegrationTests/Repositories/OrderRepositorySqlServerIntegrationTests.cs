using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using OrderFlow.Infrastructure.Repositories;
using OrderFlow.IntegrationTests.Fixtures;

namespace OrderFlow.IntegrationTests.Repositories
{
    public class OrderRepositorySqlServerIntegrationTests
        : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _databaseFixture;

        public OrderRepositorySqlServerIntegrationTests(
            DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
        }

        //[Fact]
        public async Task Should_Save_Order_And_Get_By_Id_Using_SqlServer()
        {
            // Arrange
            await using var context = _databaseFixture.CreateContext();

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