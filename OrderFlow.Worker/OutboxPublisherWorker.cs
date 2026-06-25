using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Observability;
using System.Diagnostics;

namespace OrderFlow.Worker
{
    public class OutboxPublisherWorker : BackgroundService
    {
        private readonly ILogger<OutboxPublisherWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public OutboxPublisherWorker(
            ILogger<OutboxPublisherWorker> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OutboxPublisherWorker iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var activity = Telemetry.ActivitySource.StartActivity("Outbox.PublishPendingMessages");

                try
                {
                    activity?.SetTag("worker.name", nameof(OutboxPublisherWorker));
                    activity?.SetTag("outbox.operation", "publish_pending_messages");

                    using var scope = _scopeFactory.CreateScope();

                    var publishOutboxMessagesUseCase =
                        scope.ServiceProvider.GetRequiredService<IPublishOutboxMessagesUseCase>();

                    await publishOutboxMessagesUseCase.ExecuteAsync(stoppingToken);

                    activity?.SetStatus(ActivityStatusCode.Ok);
                }
                catch (Exception ex)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    activity?.AddException(ex);

                    _logger.LogError(ex, "Erro ao publicar mensagens da outbox.");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}