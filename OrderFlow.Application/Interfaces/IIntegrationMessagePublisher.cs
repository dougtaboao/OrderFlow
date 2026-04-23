
namespace OrderFlow.Application.Interfaces
{
    public interface IIntegrationMessagePublisher
    {
        Task PublishAsync(string messageType, string payload, string correlationId, CancellationToken cancellationToken = default);
    }
}