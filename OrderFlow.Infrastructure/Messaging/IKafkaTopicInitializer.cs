namespace OrderFlow.Infrastructure.Messaging;

public interface IKafkaTopicInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
