using OrderFlow.Application.Messaging;

namespace OrderFlow.Application.Interfaces
{
    public interface IOrderMessagePublisher
    {
        Task PublishAsync(OrderCreatedMessage message, CancellationToken cancellationToken = default);
    }
}