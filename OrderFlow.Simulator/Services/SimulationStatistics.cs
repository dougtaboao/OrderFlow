using System.Diagnostics;

namespace OrderFlow.Simulator.Services;

public sealed class SimulationStatistics
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    private long _success;
    private long _failed;
    private long _completed;
    private long _totalElapsedMilliseconds;

    public void RegisterSuccess(long elapsedMilliseconds)
    {
        Interlocked.Increment(ref _success);
        Interlocked.Increment(ref _completed);
        Interlocked.Add(ref _totalElapsedMilliseconds, elapsedMilliseconds);
    }

    public void RegisterFailure(long elapsedMilliseconds)
    {
        Interlocked.Increment(ref _failed);
        Interlocked.Increment(ref _completed);
        Interlocked.Add(ref _totalElapsedMilliseconds, elapsedMilliseconds);
    }

    public long Success => Interlocked.Read(ref _success);

    public long Failed => Interlocked.Read(ref _failed);

    public long Completed => Interlocked.Read(ref _completed);

    public TimeSpan Elapsed => _stopwatch.Elapsed;

    public double OrdersPerSecond =>
        Completed == 0
            ? 0
            : Completed / _stopwatch.Elapsed.TotalSeconds;

    public double AverageMilliseconds =>
        Completed == 0
            ? 0
            : (double)_totalElapsedMilliseconds / Completed;

    public void Stop()
    {
        _stopwatch.Stop();
    }
}