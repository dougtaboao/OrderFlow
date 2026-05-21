using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using OrderFlow.Application.Messaging;
using OrderFlow.Infrastructure.Messaging;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Domain.ReadModels;

namespace OrderFlow.Worker
{
    public class KafkaOrderCompletedAuditWorker : BackgroundService
    {
        private readonly ILogger<KafkaOrderCompletedAuditWorker> _logger;
        private readonly KafkaSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;

        public KafkaOrderCompletedAuditWorker(
            ILogger<KafkaOrderCompletedAuditWorker> logger,
            KafkaSettings settings,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _settings = settings;
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken);
        }

        private async void ConsumeAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                GroupId = "orderflow-audit-consumer",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();

            consumer.Subscribe(_settings.OrderStatusChangedTopic);

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
                        JsonSerializer.Deserialize<OrderStatusChangedIntegrationEvent>(
                        result.Message.Value);

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

                    using var scope = _scopeFactory.CreateScope();

                    var repository = scope.ServiceProvider
                        .GetRequiredService<IOrderAuditReadModelRepository>();

                    var unitOfWork = scope.ServiceProvider
                        .GetRequiredService<IUnitOfWork>();

                    var readModel = new OrderAuditReadModel(
                        integrationEvent.OrderId,
                        integrationEvent.UserId,
                        integrationEvent.Amount,
                        "OrderCompleted",
                        correlationId,
                        integrationEvent.CompletedAt);

                    await repository.AddAsync(readModel, stoppingToken);

                    await unitOfWork.SaveChangesAsync(stoppingToken);

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