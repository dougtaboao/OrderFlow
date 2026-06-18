using System.Text;
using System.Text.Json;
using FluentAssertions;
using OrderFlow.Application.Messaging;
using OrderFlow.Infrastructure.Messaging;
using RabbitMQ.Client;

namespace OrderFlow.IntegrationTests.Messaging
{
    public class RabbitMqOrderMessagePublisherTests : IAsyncLifetime
    {
        private const string QueueName = "order-created-test";
        private const string DeadLetterQueueName = "order-created-dlq-test";

        private IConnection _connection = null!;
        private IChannel _channel = null!;
        private RabbitMqOrderMessagePublisher _publisher = null!;

        public async Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await _channel.QueueDeclareAsync(
                queue: DeadLetterQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await _channel.QueuePurgeAsync(QueueName);
            await _channel.QueuePurgeAsync(DeadLetterQueueName);

            var settings = new RabbitMqSettings
            {
                HostName = "localhost",
                QueueName = QueueName,
                DeadLetterQueueName = DeadLetterQueueName,
                MaxRetryCount = 3
            };

            _publisher = new RabbitMqOrderMessagePublisher(settings);
        }

        public async Task DisposeAsync()
        {
            await _channel.QueuePurgeAsync(QueueName);
            await _channel.QueuePurgeAsync(DeadLetterQueueName);

            await _channel.CloseAsync();
            await _connection.CloseAsync();

            _channel.Dispose();
            _connection.Dispose();
        }

        [Fact]
        public async Task PublishAsync_Should_Publish_OrderCreatedMessage_To_RabbitMq()
        {
            // Arrange
            var message = new OrderCreatedMessage
            {
                OrderId = Guid.NewGuid()
            };

            // Act
            await _publisher.PublishAsync(message);

            // Assert
            var result = await _channel.BasicGetAsync(
                queue: QueueName,
                autoAck: true);

            result.Should().NotBeNull();

            var json = Encoding.UTF8.GetString(result!.Body.ToArray());

            var receivedMessage = JsonSerializer.Deserialize<OrderCreatedMessage>(json);

            receivedMessage.Should().NotBeNull();
            receivedMessage!.OrderId.Should().Be(message.OrderId);
        }

        [Fact]
        public async Task PublishAsync_Should_Create_Message_With_Retry_Count_Header()
        {
            // Arrange
            var message = new OrderCreatedMessage
            {
                OrderId = Guid.NewGuid()
            };

            // Act
            await _publisher.PublishAsync(message);

            // Assert
            var result = await _channel.BasicGetAsync(
                queue: QueueName,
                autoAck: true);

            result.Should().NotBeNull();

            result!.BasicProperties.Headers.Should().NotBeNull();
            result.BasicProperties.Headers!.Should().ContainKey("x-retry-count");
        }
    }
}