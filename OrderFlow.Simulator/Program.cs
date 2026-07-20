using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OrderFlow.Simulator.Configuration;
using OrderFlow.Simulator.Scenarios;
using OrderFlow.Simulator.Services;

var host = Host.CreateDefaultBuilder(args)
    .UseContentRoot(AppContext.BaseDirectory)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(AppContext.BaseDirectory);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<SimulatorSettings>(
            context.Configuration.GetSection("Simulator"));

        services.AddHttpClient("OrderFlowApi", (serviceProvider, client) =>
        {
            var settings = serviceProvider
                .GetRequiredService<IOptions<SimulatorSettings>>()
                .Value;

            client.BaseAddress = new Uri(settings.ApiBaseUrl);
        });

        services.AddSingleton<OrderGenerator>();
        services.AddSingleton<OrderSender>();
        services.AddSingleton<ISimulationRunner, SimulationRunner>();
        services.AddHostedService<SimulationHostedService>();
        services.AddSingleton<SimulationStatistics>();
        services.AddSingleton<ProgressRenderer>();
        services.AddSingleton<ISimulationScenario, DefaultSimulationScenario>();
        services.AddSingleton<ISimulationScenario, MarketOpenSimulationScenario>();
        services.AddSingleton<SimulationScenarioResolver>();
    })
    .Build();

await host.RunAsync();

