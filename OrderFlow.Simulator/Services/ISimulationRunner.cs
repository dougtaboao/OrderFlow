namespace OrderFlow.Simulator.Services;

public interface ISimulationRunner
{
    Task RunAsync(CancellationToken cancellationToken);
}