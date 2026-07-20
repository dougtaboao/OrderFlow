namespace OrderFlow.Simulator.Services;

public sealed class ProgressRenderer
{
    public void RenderProgress(
        long completed,
        int total,
        long success,
        long failed,
        double ordersPerSecond,
        double averageMilliseconds)
    {
        var percent = total == 0
            ? 100
            : completed * 100 / total;

        var barSize = 30;
        var filled = (int)(percent * barSize / 100);

        var bar = new string('#', filled) + new string('-', barSize - filled);

        Console.Write(
            $"\r[{bar}] {percent,3}% | " +
            $"{completed}/{total} | " +
            $"OK: {success} | " +
            $"Fail: {failed} | " +
            $"Rate: {ordersPerSecond:N1}/s | " +
            $"Avg: {averageMilliseconds:N0}ms");
    }

    public void RenderFinalReport(
        int totalOrders,
        long success,
        long failed,
        TimeSpan elapsed,
        double ordersPerSecond,
        double averageMilliseconds)
    {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("======================================");
        Console.WriteLine("        SIMULAÇÃO FINALIZADA");
        Console.WriteLine("======================================");
        Console.WriteLine($"Total.............: {totalOrders}");
        Console.WriteLine($"Sucesso...........: {success}");
        Console.WriteLine($"Falha.............: {failed}");
        Console.WriteLine($"Tempo total.......: {elapsed}");
        Console.WriteLine($"Tempo médio API...: {averageMilliseconds:N2} ms");
        Console.WriteLine($"Ordens/segundo....: {ordersPerSecond:N2}");
        Console.WriteLine("======================================");
    }
}