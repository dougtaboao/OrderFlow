using Confluent.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OrderFlow.Infrastructure.Messaging;

namespace OrderFlow.Infrastructure.HealthChecks
{
    public class KafkaHealthCheck : IHealthCheck
    {
        private readonly KafkaSettings _settings;

        public KafkaHealthCheck(KafkaSettings settings)
        {
            _settings = settings;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var config = new AdminClientConfig
                {
                    BootstrapServers = _settings.BootstrapServers
                };

                using var adminClient = new AdminClientBuilder(config).Build();

                var metadata = adminClient.GetMetadata(
                    _settings.OrderCompletedTopic,
                    TimeSpan.FromSeconds(5));

                if (metadata.Topics.Any(t => t.Topic == _settings.OrderCompletedTopic))
                    return Task.FromResult(HealthCheckResult.Healthy("Kafka acessível."));

                return Task.FromResult(
                    HealthCheckResult.Unhealthy($"Tópico Kafka {_settings.OrderCompletedTopic} não encontrado."));
            }
            catch (Exception ex)
            {
                return Task.FromResult(
                    HealthCheckResult.Unhealthy("Kafka indisponível.", ex));
            }
        }
    }
}