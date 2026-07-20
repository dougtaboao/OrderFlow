namespace OrderFlow.Simulator.Configuration;

public sealed class OrderGenerationOptions
{
    public required IReadOnlyList<string> PreferredAssets { get; init; }

    public int BuyPercentage { get; init; }

    public int NormalPriorityPercentage { get; init; }

    public int HighPriorityPercentage { get; init; }

    public int CriticalPriorityPercentage { get; init; }

    public decimal PriceVariationPercent { get; init; }

    public int MinQuantityLots { get; init; }

    public int MaxQuantityLots { get; init; }
}