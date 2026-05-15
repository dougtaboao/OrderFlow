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

            using var scope = _scopeFactory.CreateScope();

            var correlationContext = scope.ServiceProvider.GetRequiredService<ICorrelationContext>();
            correlationContext.Set(correlationId);

            var processOrderUseCase = scope.ServiceProvider.GetRequiredService<IProcessOrderUseCase>();

            var receiveCount = sqsMessage.Attributes.TryGetValue("ApproximateReceiveCount", out var count)
                ? count
                : "N/A";

            try
            {
                _logger.LogInformation(
                    "Mensagem SQS recebida. MessageId {MessageId}, CorrelationId {CorrelationId}, ReceiveCount {ReceiveCount}",
                    sqsMessage.MessageId,
                    correlationId,
                    receiveCount);

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
                    "Erro ao processar mensagem SQS. MessageId {MessageId}. A mensagem não será deletada e poderá ser reenviada pelo SQS.",
                    sqsMessage.MessageId);

                // Importante:
                // Não deletar a mensagem.
                // O SQS irá reenviá-la após o Visibility Timeout.
                // Após atingir maxReceiveCount, a AWS moverá a mensagem para a DLQ configurada.
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