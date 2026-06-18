using System.Text;
using System.Text.Json;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Messaging;
using RabbitMQ.Client;

namespace OrderFlow.Infrastructure.Messaging
{
    public class RabbitMqOrderMessagePublisher : IOrderMessagePublisher
    {
        private readonly RabbitMqSettings _settings;

        public RabbitMqOrderMessagePublisher(RabbitMqSettings settings)
        {
            _settings = settings;
        }

        public async Task PublishAsync(OrderCreatedMessage message, CancellationToken cancellationToken = default)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            await using var connection = await factory.CreateConnectionAsync(cancellationToken);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: _settings.DeadLetterQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = new BasicProperties
            {
                Persistent = true,
                Headers = new Dictionary<string, object>
                {
                    { "x-retry-count", 0 }
                }
            };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: _settings.QueueName,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }
    }
}