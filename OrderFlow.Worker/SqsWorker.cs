using System.Text.Json;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Messaging;
using OrderFlow.Infrastructure.Messaging;

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
            if (!_settings.Enabled)
            {
                _logger.LogInformation("SQS Worker desabilitado.");
                return;
            }

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
                    MessageAttributeNames = new List<string> { "All" }
                }, stoppingToken);

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

            using var scope = _scopeFactory.CreateScope();

            var correlationContext = scope.ServiceProvider.GetRequiredService<ICorrelationContext>();
            correlationContext.Set(correlationId);

            var processOrderUseCase = scope.ServiceProvider.GetRequiredService<IProcessOrderUseCase>();

            try
            {
                _logger.LogInformation(
                    "Mensagem SQS recebida. MessageId {MessageId}, CorrelationId {CorrelationId}",
                    sqsMessage.MessageId,
                    correlationId);

                var message = JsonSerializer.Deserialize<OrderCreatedMessage>(sqsMessage.Body);

                if (message is null)
                {
                    _logger.LogWarning("Mensagem SQS inválida. MessageId {MessageId}", sqsMessage.MessageId);

                    await DeleteMessageAsync(client, sqsMessage, cancellationToken);
                    return;
                }

                await processOrderUseCase.ExecuteAsync(message.OrderId, cancellationToken);

                await DeleteMessageAsync(client, sqsMessage, cancellationToken);

                _logger.LogInformation(
                    "Mensagem SQS processada com sucesso. OrderId {OrderId}",
                    message.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao processar mensagem SQS. MessageId {MessageId}",
                    sqsMessage.MessageId);

                // No SQS, se não deletar a mensagem, ela volta após o Visibility Timeout.
                // A DLQ é configurada na AWS pela Redrive Policy.
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