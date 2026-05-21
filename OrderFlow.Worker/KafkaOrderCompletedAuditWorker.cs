using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using OrderFlow.Application.Messaging;
using OrderFlow.Infrastructure.Messaging;

namespace OrderFlow.Worker
{
    public class KafkaOrderCompletedAuditWorker : BackgroundService
    {
        private readonly ILogger<KafkaOrderCompletedAuditWorker> _logger;
        private readonly KafkaSettings _settings;

        public KafkaOrderCompletedAuditWorker(
            ILogger<KafkaOrderCompletedAuditWorker> logger,
            KafkaSettings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken);
        }

        private void ConsumeAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                GroupId = "orderflow-audit-consumer",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();

            consumer.Subscribe(_settings.OrderCompletedTopic);

            _logger.LogInformation(
                "Kafka audit consumer iniciado. Topic {Topic}, GroupId {GroupId}",
                _settings.OrderCompletedTopic,
                config.GroupId);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = consumer.Consume(stoppingToken);

                    var correlationId = GetHeaderValue(result.Message.Headers, "correlation-id");

                    var integrationEvent =
                        JsonSerializer.Deserialize<OrderCompletedIntegrationEvent>(result.Message.Value);

                    if (integrationEvent is null)
                    {
                        _logger.LogWarning(
                            "Evento Kafka inválido recebido. Topic {Topic}, Partition {Partition}, Offset {Offset}",
                            result.Topic,
                            result.Partition.Value,
                            result.Offset.Value);

                        consumer.Commit(result);
                        continue;
                    }

                    _logger.LogInformation(
                        "AUDIT - OrderCompleted consumido. OrderId {OrderId}, UserId {UserId}, Amount {Amount}, CorrelationId {CorrelationId}, Partition {Partition}, Offset {Offset}",
                        integrationEvent.OrderId,
                        integrationEvent.UserId,
                        integrationEvent.Amount,
                        correlationId,
                        result.Partition.Value,
                        result.Offset.Value);

                    consumer.Commit(result);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka audit consumer encerrado.");
            }
            finally
            {
                consumer.Close();
            }
        }

        private static string GetHeaderValue(Headers headers, string key)
        {
            var header = headers.FirstOrDefault(h => h.Key == key);

            if (header is null)
                return "N/A";

            return Encoding.UTF8.GetString(header.GetValueBytes());
        }
    }
}