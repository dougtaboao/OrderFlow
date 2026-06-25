using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Messaging;
using OrderFlow.Application.Observability;
using OrderFlow.Infrastructure.Messaging;
using System.Diagnostics;
using System.Text.Json;

namespace OrderFlow.Worker
{
    public class SqsWorker : BackgroundService
    {
        private readonly ILogger<SqsWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly SqsSettings _settings;

        public SqsWorker(
            ILogger<SqsWorker> logger,
            IServiceScopeFactory scopeFactory,
            SqsSettings settings)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _settings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (string.IsNullOrWhiteSpace(_settings.QueueUrl))
                throw new InvalidOperationException("QueueUrl do SQS não configurada.");

            var region = RegionEndpoint.GetBySystemName(_settings.Region);

            using var client = new AmazonSQSClient(region);

            _logger.LogInformation("SQS Worker iniciado. QueueUrl: {QueueUrl}", _settings.QueueUrl);

            while (!stoppingToken.IsCancellationRequested)
            {
                var response = await client.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    QueueUrl = _settings.QueueUrl,
                    MaxNumberOfMessages = _settings.MaxMessages,
                    WaitTimeSeconds = _settings.WaitTimeSeconds,
                    MessageAttributeNames = new List<string> { "All" },
                    AttributeNames = new List<string>
                    {
                        "ApproximateReceiveCount"
                    }
                }, stoppingToken);

                if (response.Messages is null || response.Messages.Count == 0)
                {
                    _logger.LogDebug(
                        "Nenhuma mensagem encontrada na fila SQS {QueueUrl} às {CheckedAt}",
                        _settings.QueueUrl,
                        DateTime.UtcNow);

                    continue;
                }

                foreach (var sqsMessage in response.Messages)
                {
                    await ProcessMessageAsync(client, sqsMessage, stoppingToken);
                }
            }
        }

        private async Task ProcessMessageAsync(
            IAmazonSQS client,
            Message sqsMessage,
            CancellationToken cancellationToken)
        {
            var correlationId = GetMessageAttribute(sqsMessage, "CorrelationId") ?? "N/A";

            using var activity = Telemetry.ActivitySource.StartActivity("SQS.ConsumeOrderCreated");

            activity?.SetTag("messaging.system", "aws.sqs");
            activity?.SetTag("messaging.destination", _settings.QueueUrl);
            activity?.SetTag("messaging.operation", "consume");
            activity?.SetTag("messaging.message.id", sqsMessage.MessageId);
            activity?.SetTag("correlation.id", correlationId);

            var receiveCount = sqsMessage.Attributes.TryGetValue("ApproximateReceiveCount", out var count)
                ? count
                : "N/A";

            activity?.SetTag("messaging.sqs.receive_count", receiveCount);

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                [LogProperties.CorrelationId] = correlationId,
                ["MessageId"] = sqsMessage.MessageId,
                ["ReceiveCount"] = receiveCount
            }))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var correlationContext = scope.ServiceProvider.GetRequiredService<ICorrelationContext>();
                    correlationContext.Set(correlationId);

                    var processOrderUseCase = scope.ServiceProvider.GetRequiredService<IProcessOrderUseCase>();

                    _logger.LogInformation(
                        "Mensagem SQS recebida. MessageId {MessageId}, CorrelationId {CorrelationId}, ReceiveCount {ReceiveCount}",
                        sqsMessage.MessageId,
                        correlationId,
                        receiveCount);

                    var message = JsonSerializer.Deserialize<OrderCreatedMessage>(sqsMessage.Body);

                    if (message is null)
                    {
                        activity?.SetTag("message.valid", false);
                        activity?.SetStatus(ActivityStatusCode.Error, "Invalid SQS message");

                        _logger.LogWarning(
                            "Mensagem SQS inválida. MessageId {MessageId}",
                            sqsMessage.MessageId);

                        await DeleteMessageAsync(client, sqsMessage, cancellationToken);

                        return;
                    }

                    activity?.SetTag("message.valid", true);
                    activity?.SetTag("order.id", message.OrderId);

                    await processOrderUseCase.ExecuteAsync(message.OrderId, cancellationToken);

                    await DeleteMessageAsync(client, sqsMessage, cancellationToken);

                    activity?.SetStatus(ActivityStatusCode.Ok);

                    _logger.LogInformation(
                        "Mensagem SQS processada com sucesso. OrderId {OrderId}",
                        message.OrderId);
                }
                catch (Exception ex)
                {
                    Metrics.OrdersFailed.Add(1);

                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    activity?.AddException(ex);

                    _logger.LogError(
                        ex,
                        "Erro ao processar mensagem SQS. MessageId {MessageId}. A mensagem não será deletada e poderá ser reenviada pelo SQS.",
                        sqsMessage.MessageId);
                }
            }
        }

        private async Task DeleteMessageAsync(
            IAmazonSQS client,
            Message sqsMessage,
            CancellationToken cancellationToken)
        {
            await client.DeleteMessageAsync(new DeleteMessageRequest
            {
                QueueUrl = _settings.QueueUrl,
                ReceiptHandle = sqsMessage.ReceiptHandle
            }, cancellationToken);
        }

        private static string? GetMessageAttribute(Message message, string name)
        {
            if (!message.MessageAttributes.TryGetValue(name, out var attribute))
                return null;

            return attribute.StringValue;
        }
    }
}