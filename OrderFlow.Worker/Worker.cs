using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Messaging;
using OrderFlow.Application.Observability;
using OrderFlow.Infrastructure.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace OrderFlow.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RabbitMqSettings _settings;

        private IConnection? _connection;
        private IChannel? _channel;

        public Worker(
            ILogger<Worker> logger,
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;

            _settings = configuration
                .GetSection("RabbitMq")
                .Get<RabbitMqSettings>() ?? new RabbitMqSettings();
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando conexão com RabbitMQ em {Host}", _settings.HostName);

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(
                queue: _settings.DeadLetterQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: cancellationToken);

            _logger.LogInformation("RabbitMQ conectado. Fila principal e DLQ prontas.");

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel is null)
                throw new InvalidOperationException("Channel RabbitMQ não foi inicializado.");

            _logger.LogInformation("Worker pronto para consumir mensagens da fila {QueueName}", _settings.QueueName);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                var correlationId = ea.BasicProperties?.CorrelationId ?? "N/A";

                using var activity = Telemetry.ActivitySource.StartActivity("RabbitMQ.ConsumeOrderCreated");

                activity?.SetTag("messaging.system", "rabbitmq");
                activity?.SetTag("messaging.destination", _settings.QueueName);
                activity?.SetTag("messaging.operation", "consume");
                activity?.SetTag("messaging.rabbitmq.delivery_tag", ea.DeliveryTag);
                activity?.SetTag("correlation.id", correlationId);

                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    [LogProperties.CorrelationId] = correlationId,
                    [LogProperties.QueueName] = _settings.QueueName,
                    ["DeliveryTag"] = ea.DeliveryTag
                }))
                {
                    try
                    {
                        _logger.LogInformation(
                            "{Event} - Mensagem recebida do RabbitMQ",
                            LogEvents.RabbitMessageReceived);

                        var body = ea.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);

                        _logger.LogDebug("Payload recebido: {Payload}", json);

                        var message = JsonSerializer.Deserialize<OrderCreatedMessage>(json);

                        if (message is null)
                        {
                            activity?.SetTag("message.valid", false);
                            activity?.SetStatus(ActivityStatusCode.Error, "Invalid RabbitMQ message");

                            _logger.LogWarning("Mensagem inválida recebida.");

                            await _channel.BasicAckAsync(
                                ea.DeliveryTag,
                                multiple: false,
                                cancellationToken: stoppingToken);

                            return;
                        }

                        activity?.SetTag("message.valid", true);
                        activity?.SetTag("order.id", message.OrderId);

                        using var scope = _scopeFactory.CreateScope();

                        var correlationContext = scope.ServiceProvider.GetRequiredService<ICorrelationContext>();
                        correlationContext.Set(correlationId);

                        var processOrderUseCase = scope.ServiceProvider.GetRequiredService<IProcessOrderUseCase>();

                        await processOrderUseCase.ExecuteAsync(message.OrderId, stoppingToken);

                        await _channel.BasicAckAsync(
                            ea.DeliveryTag,
                            multiple: false,
                            cancellationToken: stoppingToken);

                        activity?.SetStatus(ActivityStatusCode.Ok);

                        _logger.LogInformation(
                            "Mensagem processada com sucesso para OrderId {OrderId}",
                            message.OrderId);
                    }
                    catch (Exception ex)
                    {
                        activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                        activity?.AddException(ex);

                        _logger.LogError(ex, "Erro ao processar mensagem.");

                        var retryCount = GetRetryCount(ea.BasicProperties);

                        if (retryCount < _settings.MaxRetryCount)
                        {
                            activity?.SetTag("messaging.retry", true);
                            activity?.SetTag("messaging.retry.count", retryCount + 1);

                            await RepublishWithRetryAsync(ea, retryCount + 1, stoppingToken);

                            _logger.LogWarning(
                                 "{Event} - Mensagem reenfileirada para retry {RetryCount}",
                                 LogEvents.MessageRetried,
                                 retryCount + 1);
                        }
                        else
                        {
                            activity?.SetTag("messaging.dlq", true);
                            activity?.SetTag("messaging.retry.max_reached", true);

                            await PublishToDeadLetterQueueAsync(ea, stoppingToken);

                            Metrics.OrdersFailed.Add(1);

                            _logger.LogError(
                                "{Event} - Mensagem enviada para DLQ após {RetryCount} tentativas",
                                LogEvents.MessageSentToDlq,
                                retryCount);
                        }

                        await _channel.BasicAckAsync(
                            ea.DeliveryTag,
                            multiple: false,
                            cancellationToken: stoppingToken);
                    }
                }
            };

            await _channel.BasicConsumeAsync(
                queue: _settings.QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation("Consumer registrado com sucesso.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private int GetRetryCount(IReadOnlyBasicProperties? properties)
        {
            if (properties?.Headers is null)
                return 0;

            if (!properties.Headers.TryGetValue("x-retry-count", out var value))
                return 0;

            return value switch
            {
                byte[] bytes when int.TryParse(Encoding.UTF8.GetString(bytes), out var result) => result,
                int intValue => intValue,
                long longValue => (int)longValue,
                _ => 0
            };
        }

        private async Task RepublishWithRetryAsync(
            BasicDeliverEventArgs ea,
            int retryCount,
            CancellationToken cancellationToken)
        {
            if (_channel is null)
                return;

            var properties = new BasicProperties
            {
                Persistent = true,
                CorrelationId = ea.BasicProperties?.CorrelationId,
                Headers = new Dictionary<string, object>
                {
                    { "x-retry-count", retryCount }
                }
            };

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: _settings.QueueName,
                mandatory: false,
                basicProperties: properties,
                body: ea.Body,
                cancellationToken: cancellationToken);
        }

        private async Task PublishToDeadLetterQueueAsync(
            BasicDeliverEventArgs ea,
            CancellationToken cancellationToken)
        {
            if (_channel is null)
                return;

            var properties = new BasicProperties
            {
                Persistent = true,
                CorrelationId = ea.BasicProperties?.CorrelationId,
                Headers = ea.BasicProperties?.Headers is null
                    ? new Dictionary<string, object>()
                    : new Dictionary<string, object>(ea.BasicProperties.Headers)
            };

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: _settings.DeadLetterQueueName,
                mandatory: false,
                basicProperties: properties,
                body: ea.Body,
                cancellationToken: cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel is not null)
                await _channel.CloseAsync(cancellationToken);

            if (_connection is not null)
                await _connection.CloseAsync(cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}