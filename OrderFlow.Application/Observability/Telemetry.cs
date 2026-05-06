using System.Diagnostics;

namespace OrderFlow.Application.Observability
{
    public static class Telemetry
    {
        public static readonly ActivitySource ActivitySource = new("OrderFlow");
    }
}