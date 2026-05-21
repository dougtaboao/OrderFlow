using OrderFlow.Application.Messaging;

namespace OrderFlow.Application.Interfaces
{
    public interface IOrderEventPublisher
    {
        Task PublishOrderCompletedAsync(OrderCompletedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);

        Task PublishOrderStatusChangedAsync(OrderStatusChangedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
    }
}