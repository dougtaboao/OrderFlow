using OrderFlow.Application.Messaging;

namespace OrderFlow.Application.Interfaces
{
    public interface IOrderEventPublisher
    {
        Task PublishOrderCompletedAsync(OrderCompletedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
    }
}