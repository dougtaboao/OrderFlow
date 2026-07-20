using OrderFlow.Simulator.Configuration;

namespace OrderFlow.Simulator.Scenarios;

public sealed class DefaultSimulationScenario : ISimulationScenario
{
    public string Name => "Default";

    public int GetConcurrency(int configured)
        => configured;

    public int GetTotalOrders(int configured)
        => configured;

    public TimeSpan GetDelay(TimeSpan configuredDelay)
        => configuredDelay;

    public OrderGenerationOptions GetGenerationOptions()
    {
        return new()
        {
            PreferredAssets =
            [
                "PETR4",
                "VALE3",
                "ITUB4",
                "BBDC4",
                "ABEV3",
                "BBAS3",
                "WEGE3",
                "SUZB3",
                "RENT3",
                "PRIO3"
            ],

            BuyPercentage = 70,

            NormalPriorityPercentage = 70,

            HighPriorityPercentage = 20,

            CriticalPriorityPercentage = 10,

            PriceVariationPercent = 2,

            MinQuantityLots = 1,

            MaxQuantityLots = 100
        };
    }
}