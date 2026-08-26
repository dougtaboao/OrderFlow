using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using OrderFlow.Application.Messaging;
using OrderFlow.Infrastructure.Messaging;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Domain.ReadModels;
using OrderFlow.Application.Observability;

namespace OrderFlow.Worker
{
    public class KafkaOrderStatusChangedAuditWorker : BackgroundService
    {
        private readonly ILogger<KafkaOrderStatusChangedAuditWorker> _logger;
        private readonly KafkaSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;

        public KafkaOrderStatusChangedAuditWorker(
            ILogger<KafkaOrderStatusChangedAuditWorker> logger,
            KafkaSettings settings,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _settings = settings;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                GroupId = "orderflow-status-audit-consumer",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();

            consumer.Subscribe(_settings.OrderStatusChangedTopic);

            _logger.LogInformation(
                "Kafka status changed audit consumer iniciado. Topic {Topic}, GroupId {GroupId}",
                _settings.OrderStatusChangedTopic,
                config.GroupId);

            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<string, string>? result = null;
                try
                {
                    result = consumer.Consume(stoppingToken);

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
                        integrationEvent.NewStatus,
                        correlationId,
                        integrationEvent.OccurredAt);

                    await repository.AddAsync(readModel, stoppingToken);

                    await unitOfWork.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation(
                        "AUDIT - OrderStatusChanged consumido. OrderId {OrderId}, PreviousStatus {PreviousStatus}, NewStatus {NewStatus}, CorrelationId {CorrelationId}, Partition {Partition}, Offset {Offset}",
                        integrationEvent.OrderId,
                        integrationEvent.PreviousStatus,
                        integrationEvent.NewStatus,
                        correlationId,
                        result.Partition.Value,
                        result.Offset.Value);

                    consumer.Commit(result);
                    Metrics.KafkaAuditEventsConsumed.Add(1);
                }
                catch (ConsumeException ex)
                {
                    Metrics.KafkaConsumerErrors.Add(1);
                    _logger.LogError(ex, "Falha ao consumir Kafka. Code {Code}, Reason {Reason}", ex.Error.Code, ex.Error.Reason);
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }
                catch (JsonException ex)
                {
                    Metrics.KafkaConsumerErrors.Add(1);
                    _logger.LogError(ex, "Payload inválido recebido do Kafka; offset confirmado para evitar poison loop.");
                    if (result is not null)
                        consumer.Commit(result);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado no consumer de auditoria Kafka.");
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }
            }

            consumer.Close();
            _logger.LogInformation("Kafka audit consumer encerrado.");
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
