using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Messaging;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.CircuitBreaker;

namespace OrderFlow.Infrastructure.Messaging
{
    public class KafkaOrderEventPublisher : IOrderEventPublisher
    {
        private readonly KafkaSettings _settings;
        private readonly AsyncTimeoutPolicy _timeoutPolicy;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public KafkaOrderEventPublisher(KafkaSettings settings)
        {
            _settings = settings;

            _timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromSeconds(5));

            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30));
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

            var policy = Policy.WrapAsync(
                _circuitBreakerPolicy,
                _retryPolicy,
                _timeoutPolicy);

            await policy.ExecuteAsync(async () =>
            {
                await producer.ProduceAsync(
                    _settings.OrderCompletedTopic,
                    message,
                    cancellationToken);
            });
        }

        public async Task PublishOrderStatusChangedAsync(
            OrderStatusChangedIntegrationEvent integrationEvent,
            CancellationToken cancellationToken = default)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _settings.BootstrapServers
            };

            using var producer =
                new ProducerBuilder<string, string>(config).Build();

            var message = new Message<string, string>
            {
                Key = integrationEvent.OrderId.ToString(),
                Value = JsonSerializer.Serialize(integrationEvent),
                Headers = new Headers()
            };

            message.Headers.Add(
                "correlation-id",
                Encoding.UTF8.GetBytes(integrationEvent.CorrelationId));

            await producer.ProduceAsync(
                _settings.OrderStatusChangedTopic,
                message,
                cancellationToken);
        }
    }
}