using OrderFlow.Simulator.Configuration;

namespace OrderFlow.Simulator.Scenarios;

public sealed class MarketOpenSimulationScenario : ISimulationScenario
{
    public string Name => "MarketOpen";

    public int GetConcurrency(int configured)
        => Math.Max(configured, 25);

    public int GetTotalOrders(int configured)
        => Math.Max(configured, 1000);

    public TimeSpan GetDelay(TimeSpan configuredDelay)
        => TimeSpan.Zero;

    public OrderGenerationOptions GetGenerationOptions()
    {
        return new()
        {
            PreferredAssets =
            [
                "PETR4",
                "VALE3",
                "ITUB4"
            ],

            BuyPercentage = 80,

            NormalPriorityPercentage = 50,

            HighPriorityPercentage = 35,

            CriticalPriorityPercentage = 15,

            PriceVariationPercent = 5,

            MinQuantityLots = 20,

            MaxQuantityLots = 500
        };
    }
}