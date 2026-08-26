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

                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));

                var requiredTopics = new[]
                {
                    _settings.OrderCompletedTopic,
                    _settings.OrderStatusChangedTopic
                };

                var missingTopics = requiredTopics
                    .Where(required => metadata.Topics.All(topic => topic.Topic != required || topic.Error.Code != ErrorCode.NoError))
                    .ToArray();

                if (missingTopics.Length == 0)
                    return Task.FromResult(HealthCheckResult.Healthy("Kafka e tópicos obrigatórios acessíveis."));

                return Task.FromResult(
                    HealthCheckResult.Unhealthy($"Tópicos Kafka ausentes: {string.Join(", ", missingTopics)}."));
            }
            catch (Exception ex)
            {
                return Task.FromResult(
                    HealthCheckResult.Unhealthy("Kafka indisponível.", ex));
            }
        }
    }
}
