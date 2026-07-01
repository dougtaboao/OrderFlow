using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Dtos;
using OrderFlow.Application.Messaging;
using OrderFlow.Application.Security;
using OrderFlow.Domain.Enums;
using OrderFlow.Infrastructure.Data;
using OrderFlow.IntegrationTests.Fixtures;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OrderFlow.IntegrationTests.Api
{
    public class CreateOrderApiIntegrationTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _integrationTestFixture;

        public CreateOrderApiIntegrationTests(IntegrationTestFixture integrationTestFixture)
        {
            _integrationTestFixture = integrationTestFixture;
        }

        [Fact]
        public async Task Post_Orders_Should_Create_Order()
        {
            await _integrationTestFixture.ResetAsync();

            // Arrange
            await using var apiFactory = new OrderFlowApiFactory(_integrationTestFixture);
            var client = apiFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Test");

            var request = new CreateOrderRequest
            {
                Amount = 100m,
                Type = OrderType.Buy,
                Priority = OrderPriority.Normal,
                ExternalReference = $"EXT-API-{Guid.NewGuid()}",
                AssetCode = "PETR4",
                Quantity = 10,
                UnitPrice = 10m
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/orders", request);

            // Assert

            var body = await response.Content.ReadAsStringAsync();

            Assert.True(
                response.StatusCode == HttpStatusCode.Created,
                $"Expected Created but got {response.StatusCode}. Body: {body}"
            );


            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            await using var context = _integrationTestFixture.CreateContext();

            var order = await context.Orders
                .FirstOrDefaultAsync(x => x.ExternalReference == request.ExternalReference);

            Assert.NotNull(order);
            Assert.Equal(request.Amount, order!.Amount);
            Assert.Equal(request.Type, order.Type);
            Assert.Equal(request.Priority, order.Priority);
            Assert.Equal(request.AssetCode, order.AssetCode);
        }

        [Fact]
        public async Task Should_Create_Order_And_Get_By_Id()
        {
            await _integrationTestFixture.ResetAsync();

            // Arrange
            await using var apiFactory = new OrderFlowApiFactory(_integrationTestFixture);

            var client = apiFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");

            var request = new CreateOrderRequest
            {
                Amount = 250m,
                Type = OrderType.Buy,
                Priority = OrderPriority.High,
                ExternalReference = $"EXT-{Guid.NewGuid()}",
                AssetCode = "PETR4",
                Quantity = 10,
                UnitPrice = 25m
            };

            // Act 1 - POST

            var postResponse =
                await client.PostAsJsonAsync("/api/orders", request);

            postResponse.EnsureSuccessStatusCode();

            var created =
                await postResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();

            // Act 2 - GET

            var getResponse =
                await client.GetAsync($"/api/orders/{created!.OrderId}");

            // Assert

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var order =
                await getResponse.Content.ReadFromJsonAsync<GetOrderByIdResponse>();

            Assert.NotNull(order);

            Assert.Equal(request.Amount, order!.Amount);
            Assert.Equal(request.Type.ToString(), order.Type);
            Assert.Equal(request.Priority.ToString(), order.Priority);
            Assert.Equal(request.AssetCode, order.AssetCode);
        }

        [Fact]
        public async Task Get_Order_By_Id_Should_Return_NotFound_When_Order_Does_Not_Exist()
        {
            await _integrationTestFixture.ResetAsync();

            // Arrange
            await using var apiFactory = new OrderFlowApiFactory(_integrationTestFixture);

            var client = apiFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");

            var nonExistingOrderId = Guid.NewGuid();

            // Act
            var response = await client.GetAsync($"/api/orders/{nonExistingOrderId}");

            // Assert

            var body = await response.Content.ReadAsStringAsync();

            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound,
                $"Expected NotFound but got {response.StatusCode}. Body: {body}"
            );
        }

        [Fact]
        public async Task Post_Order_Should_Return_Unauthorized_When_User_Is_Not_Authenticated()
        {
            await _integrationTestFixture.ResetAsync();

            // Arrange
            await using var apiFactory = new OrderFlowApiFactory(_integrationTestFixture);

            var client = apiFactory.CreateClient();

            var request = new CreateOrderRequest
            {
                Amount = 100m,
                Type = OrderType.Buy,
                Priority = OrderPriority.Normal,
                ExternalReference = $"EXT-UNAUTH-{Guid.NewGuid()}",
                AssetCode = "PETR4",
                Quantity = 10,
                UnitPrice = 10m
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/orders", request);

            // Assert

            var body = await response.Content.ReadAsStringAsync();

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized,
                $"Expected Unauthorized but got {response.StatusCode}. Body: {body}"
            );
        }

        [Fact]
        public async Task Post_Order_Should_Return_BadRequest_When_Amount_Is_Invalid()
        {
            await _integrationTestFixture.ResetAsync();

            // Arrange
            await using var apiFactory = new OrderFlowApiFactory(_integrationTestFixture);

            var client = apiFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");

            var request = new CreateOrderRequest
            {
                Amount = -10m,
                Type = OrderType.Buy,
                Priority = OrderPriority.Normal,
                ExternalReference = $"EXT-INVALID-{Guid.NewGuid()}",
                AssetCode = "PETR4",
                Quantity = 10,
                UnitPrice = 10m
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/orders", request);

            // Assert
            var body = await response.Content.ReadAsStringAsync();

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest,
                $"Expected BadRequest but got {response.StatusCode}. Body: {body}"
            );
        }

        [Fact]
        public async Task Get_Order_By_Id_Should_Return_Unauthorized_When_User_Is_Not_Authenticated()
        {
            await _integrationTestFixture.ResetAsync();

            // Arrange
            await using var apiFactory = new OrderFlowApiFactory(_integrationTestFixture);

            var client = apiFactory.CreateClient();

            var orderId = Guid.NewGuid();

            // Act
            var response = await client.GetAsync($"/api/orders/{orderId}");

            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized,
                $"Status real: {response.StatusCode}. Body: {body}");
        }

        [Fact]
        public async Task Post_Order_Should_Create_Order_And_Outbox()
        {
            await _integrationTestFixture.ResetAsync();

            // Arrange
            await using var apiFactory = new OrderFlowApiFactory(_integrationTestFixture);

            var client = apiFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");

            var request = new CreateOrderRequest
            {
                Amount = 300m,
                Type = OrderType.Buy,
                Priority = OrderPriority.High,
                ExternalReference = $"EXT-OUTBOX-{Guid.NewGuid()}",
                AssetCode = "PETR4",
                Quantity = 10,
                UnitPrice = 30m
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/orders", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created =
                await response.Content.ReadFromJsonAsync<CreateOrderResponse>();

            Assert.NotNull(created);

            await using var context = _integrationTestFixture.CreateContext();

            var order = await context.Orders
                .FirstOrDefaultAsync(x => x.Id == created!.OrderId);

            var outbox = await context.OutboxMessages
                .FirstOrDefaultAsync(x => x.Payload.Contains(created!.OrderId.ToString()));

            Assert.NotNull(order);
            Assert.NotNull(outbox);

            Assert.Equal(request.ExternalReference, order!.ExternalReference);
            Assert.Equal(nameof(OrderCreatedMessage), outbox!.Type);
        }

        [Fact]
        public async Task Post_Order_Should_Return_Forbidden_When_User_Does_Not_Have_CreateOrder_Role()
        {
            await _integrationTestFixture.ResetAsync();

            // Arrange
            await using var apiFactory = new OrderFlowApiFactory(_integrationTestFixture);

            var client = apiFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");

            client.DefaultRequestHeaders.Add("X-Test-Roles", Roles.Viewer);

            var request = new CreateOrderRequest
            {
                Amount = 100m,
                Type = OrderType.Buy,
                Priority = OrderPriority.Normal,
                ExternalReference = $"EXT-FORBIDDEN-{Guid.NewGuid()}",
                AssetCode = "PETR4",
                Quantity = 10,
                UnitPrice = 10m
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/orders", request);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}