using OrderFlow.Application.Dtos;
using OrderFlow.Domain.Enums;
using OrderFlow.Simulator.Configuration;

namespace OrderFlow.Simulator.Services;

public sealed class OrderGenerator
{
    private readonly Dictionary<string, decimal> _basePrices = new()
    {
        ["PETR4"] = 32.50m,
        ["VALE3"] = 68.40m,
        ["ITUB4"] = 37.10m,
        ["BBDC4"] = 15.80m,
        ["ABEV3"] = 13.90m,
        ["BBAS3"] = 29.60m,
        ["WEGE3"] = 42.80m,
        ["SUZB3"] = 57.20m,
        ["RENT3"] = 44.50m,
        ["PRIO3"] = 49.80m
    };

    public CreateOrderRequest Generate(
        int sequence,
        OrderGenerationOptions options)
    {
        var asset = PickAsset(options);

        var unitPrice = PickPrice(asset, options);

        var quantity = PickQuantity(options);

        var orderType = PickOrderType(options);

        var priority = PickPriority(options);

        return new CreateOrderRequest
        {
            ExternalReference =
                $"SIM-{DateTime.UtcNow:yyyyMMdd}-{sequence:D6}",

            AssetCode = asset,

            UnitPrice = unitPrice,

            Quantity = quantity,

            Amount = unitPrice * quantity,

            Type = orderType,

            Priority = priority
        };
    }

    private string PickAsset(OrderGenerationOptions options)
    {
        return options.PreferredAssets[
            Random.Shared.Next(options.PreferredAssets.Count)];
    }

    private int PickQuantity(OrderGenerationOptions options)
    {
        return Random.Shared.Next(
            options.MinQuantityLots,
            options.MaxQuantityLots + 1);
    }

    private OrderType PickOrderType(OrderGenerationOptions options)
    {
        return Random.Shared.Next(100)
            < options.BuyPercentage
                ? OrderType.Buy
                : OrderType.Sell;
    }

    private OrderPriority PickPriority(OrderGenerationOptions options)
    {
        var value = Random.Shared.Next(100);

        if (value < options.NormalPriorityPercentage)
            return OrderPriority.Normal;

        if (value <
            options.NormalPriorityPercentage +
            options.HighPriorityPercentage)
            return OrderPriority.High;

        return OrderPriority.Low;
    }

    private decimal PickPrice(
        string asset,
        OrderGenerationOptions options)
    {
        var basePrice = _basePrices[asset];

        var variation =
            (decimal)Random.Shared.NextDouble();

        variation *= options.PriceVariationPercent;

        variation /= 100m;

        if (Random.Shared.Next(2) == 0)
            variation *= -1;

        return Math.Round(
            basePrice * (1 + variation),
            2);
    }
}