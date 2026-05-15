using Microsoft.Extensions.Diagnostics.HealthChecks;
using OrderFlow.Infrastructure.Messaging;
using RabbitMQ.Client;

namespace OrderFlow.Infrastructure.HealthChecks
{
    public class RabbitMqHealthCheck : IHealthCheck
    {
        private readonly RabbitMqSettings _settings;

        public RabbitMqHealthCheck(RabbitMqSettings settings)
        {
            _settings = settings;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _settings.HostName
                };

                await using var connection = await factory.CreateConnectionAsync(cancellationToken);
                await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

                return HealthCheckResult.Healthy("RabbitMQ acessível.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("RabbitMQ indisponível.", ex);
            }
        }
    }
}