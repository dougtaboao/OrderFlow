using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Messaging;

namespace OrderFlow.Infrastructure.Messaging
{
    public class KafkaOrderEventPublisher : IOrderEventPublisher
    {
        private readonly KafkaSettings _settings;

        public KafkaOrderEventPublisher(KafkaSettings settings)
        {
            _settings = settings;
        }

        public async Task PublishOrderCompletedAsync(OrderCompletedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _settings.BootstrapServers
            };

            using var producer = new ProducerBuilder<string, string>(config).Build();

            var message = new Message<string, string>
            {
                Key = integrationEvent.OrderId.ToString(),
                Value = JsonSerializer.Serialize(integrationEvent),
                Headers = new Headers()
            };

            message.Headers.Add("correlation-id", Encoding.UTF8.GetBytes(integrationEvent.CorrelationId));

            await producer.ProduceAsync(
                _settings.OrderCompletedTopic,
                message,
                cancellationToken);
        }
    }
}