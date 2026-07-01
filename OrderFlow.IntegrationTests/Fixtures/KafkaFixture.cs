using Testcontainers.Kafka;

namespace OrderFlow.IntegrationTests.Fixtures;

public sealed class KafkaFixture : IAsyncLifetime
{
    public KafkaContainer Container { get; } =
        new KafkaBuilder("confluentinc/cp-kafka:7.6.1")
            // .WithImage("confluentinc/cp-kafka:7.6.1")
            .Build();

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}