using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderFlow.Simulator.Configuration;
using OrderFlow.Simulator.Scenarios;

namespace OrderFlow.Simulator.Services;

public sealed class SimulationRunner : ISimulationRunner
{
    private readonly ILogger<SimulationRunner> _logger;
    private readonly SimulatorSettings _settings;
    private readonly OrderGenerator _generator;
    private readonly OrderSender _sender;
    private readonly SimulationStatistics _statistics;
    private readonly ProgressRenderer _progressRenderer;
    private readonly SimulationScenarioResolver _scenarioResolver;

    public SimulationRunner(
        ILogger<SimulationRunner> logger,
        IOptions<SimulatorSettings> settings,
        OrderGenerator generator,
        OrderSender sender,
        SimulationStatistics statistics,
        ProgressRenderer progressRenderer,
        SimulationScenarioResolver scenarioResolver)
    {
        _logger = logger;
        _settings = settings.Value;
        _generator = generator;
        _sender = sender;
        _statistics = statistics;
        _progressRenderer = progressRenderer;
        _scenarioResolver = scenarioResolver;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando simulação...");

        var scenario = _scenarioResolver.Resolve(_settings.Scenario);

        var totalOrders = scenario.GetTotalOrders(_settings.TotalOrders);

        var concurrency = scenario.GetConcurrency(_settings.Concurrency);

        var delay = scenario.GetDelay(
            TimeSpan.FromMilliseconds(_settings.DelayBetweenBatchesMilliseconds));

        var generationOptions = scenario.GetGenerationOptions();

        _logger.LogInformation(
            "Cenário selecionado: {Scenario}. TotalOrders: {TotalOrders}. Concurrency: {Concurrency}. Delay: {Delay}",
            scenario.Name,
            totalOrders,
            concurrency,
            delay);

        using var semaphore = new SemaphoreSlim(concurrency);

        var tasks = Enumerable.Range(1, totalOrders)
            .Select(async sequence =>
            {
                await semaphore.WaitAsync(cancellationToken);

                try
                {
                    var order = _generator.Generate(
                        sequence,
                        generationOptions);

                    var orderStopwatch = Stopwatch.StartNew();

                    var sent = await _sender.SendAsync(
                        order,
                        cancellationToken);

                    orderStopwatch.Stop();

                    if (sent)
                    {
                        _statistics.RegisterSuccess(orderStopwatch.ElapsedMilliseconds);
                    }
                    else
                    {
                        _statistics.RegisterFailure(orderStopwatch.ElapsedMilliseconds);
                    }

                    PrintProgress(totalOrders);
                }
                catch (Exception ex)
                {
                    _statistics.RegisterFailure(0);

                    _logger.LogError(
                        ex,
                        "Erro inesperado ao enviar ordem simulada.");

                    PrintProgress(totalOrders);
                }
                finally
                {
                    semaphore.Release();
                }

                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, cancellationToken);
                }
            });

        await Task.WhenAll(tasks);

        _statistics.Stop();

        PrintFinalReport(totalOrders);
    }

    private void PrintProgress(int totalOrders)
    {
        _progressRenderer.RenderProgress(
            _statistics.Completed,
            totalOrders,
            _statistics.Success,
            _statistics.Failed,
            _statistics.OrdersPerSecond,
            _statistics.AverageMilliseconds);
    }

    private void PrintFinalReport(int totalOrders)
    {
        _progressRenderer.RenderFinalReport(
            totalOrders,
            _statistics.Success,
            _statistics.Failed,
            _statistics.Elapsed,
            _statistics.OrdersPerSecond,
            _statistics.AverageMilliseconds);
    }
}