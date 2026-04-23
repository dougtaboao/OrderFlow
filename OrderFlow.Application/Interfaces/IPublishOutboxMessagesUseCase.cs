namespace OrderFlow.Application.Interfaces
{
    public interface IPublishOutboxMessagesUseCase
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}