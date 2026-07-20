namespace OrderFlow.Simulator.Configuration;

public sealed class SimulatorSettings
{
    public string ApiBaseUrl { get; set; } = "http://localhost:5000";
    public string BearerToken { get; set; } = "";

    public int TotalOrders { get; set; } = 100;
    public int Concurrency { get; set; } = 10;
    public int DelayBetweenBatchesMilliseconds { get; set; } = 500;
    public string Scenario { get; set; } = "Default";
}