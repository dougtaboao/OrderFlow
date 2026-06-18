using Testcontainers.RabbitMq;

namespace OrderFlow.IntegrationTests.Fixtures;

public sealed class RabbitMqFixture : IAsyncLifetime
{
    public const string UserName = "guest";
    public const string Password = "guest";

    public RabbitMqContainer Container { get; } =
        new RabbitMqBuilder()
            .WithImage("rabbitmq:3-management")
            .WithUsername(UserName)
            .WithPassword(Password)
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