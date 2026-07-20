using Microsoft.Extensions.Hosting;
using OrderFlow.Simulator.Services;

public sealed class SimulationHostedService : IHostedService
{
    private readonly ISimulationRunner _runner;

    public SimulationHostedService(ISimulationRunner runner)
    {
        _runner = runner;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _runner.RunAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}