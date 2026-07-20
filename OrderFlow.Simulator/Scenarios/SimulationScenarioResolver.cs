namespace OrderFlow.Simulator.Scenarios;

public sealed class SimulationScenarioResolver
{
    private readonly IEnumerable<ISimulationScenario> _scenarios;

    public SimulationScenarioResolver(IEnumerable<ISimulationScenario> scenarios)
    {
        _scenarios = scenarios;
    }

    public ISimulationScenario Resolve(string scenarioName)
    {
        return _scenarios.FirstOrDefault(s =>
            s.Name.Equals(scenarioName, StringComparison.OrdinalIgnoreCase))
            ?? _scenarios.First(s => s.Name == "Default");
    }
}