using OrderFlow.Simulator.Configuration;

namespace OrderFlow.Simulator.Scenarios;

public interface ISimulationScenario
{
    string Name { get; }

    int GetConcurrency(int configured);

    int GetTotalOrders(int configured);

    TimeSpan GetDelay(TimeSpan configuredDelay);

    OrderGenerationOptions GetGenerationOptions();
}