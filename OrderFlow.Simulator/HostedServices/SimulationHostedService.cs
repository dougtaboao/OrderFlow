using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using OrderFlow.Simulator.Services;

public sealed class SimulationHostedService : BackgroundService
{
    private readonly ISimulationRunner _runner;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public SimulationHostedService(
        ISimulationRunner runner,
        IHostApplicationLifetime applicationLifetime)
    {
        _runner = runner;
        _applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        try
        {
            await _runner.RunAsync(stoppingToken);
        }
        finally
        {
            _applicationLifetime.StopApplication();
        }
    }
}