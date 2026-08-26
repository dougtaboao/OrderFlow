using Microsoft.Extensions.Logging;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Observability;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.UseCases
{
    public class PublishOutboxMessagesUseCase : IPublishOutboxMessagesUseCase
    {
        private readonly IOutboxMessageRepository _outboxMessageRepository;
        private readonly IIntegrationMessagePublisher _integrationMessagePublisher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PublishOutboxMessagesUseCase> _logger;

        public PublishOutboxMessagesUseCase(
            IOutboxMessageRepository outboxMessageRepository,
            IIntegrationMessagePublisher integrationMessagePublisher,
            IUnitOfWork unitOfWork,
            ILogger<PublishOutboxMessagesUseCase> logger)
        {
            _outboxMessageRepository = outboxMessageRepository;
            _integrationMessagePublisher = integrationMessagePublisher;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var pendingMessages = await _outboxMessageRepository.GetPendingMessagesAsync(20, cancellationToken);

            if (pendingMessages.Count == 0)
            {
                _logger.LogDebug("Nenhuma mensagem pendente na outbox.");
                return;
            }

            _logger.LogInformation("Encontradas {Count} mensagens pendentes na outbox.", pendingMessages.Count);

            foreach (var message in pendingMessages)
            {
                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    [LogProperties.CorrelationId] = message.CorrelationId,
                    [LogProperties.OutboxMessageId] = message.Id
                }))
                {
                    try
                    {
                        _logger.LogInformation(
                            "{Event} - Publicando mensagem da outbox do tipo {MessageType}",
                            LogEvents.OutboxPublishingStarted,
                            message.Type);

                        await _integrationMessagePublisher.PublishAsync(
                            message.Type,
                            message.Payload,
                            message.CorrelationId,
                            cancellationToken);

                        message.MarkAsProcessed();

                        _logger.LogInformation(
                            "{Event} - Mensagem da outbox publicada com sucesso",
                            LogEvents.OutboxMessagePublished);
                    }
                    catch (Exception ex)
                    {
                        message.MarkAsFailed(ex.Message);

                        _logger.LogError(
                            ex,
                            "{Event} - Falha ao publicar mensagem da outbox",
                            LogEvents.OrderProcessingFailed);
                    }

                    await _outboxMessageRepository.Update(message);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}