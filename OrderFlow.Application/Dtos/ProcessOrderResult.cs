namespace OrderFlow.Application.Dtos
{
    public class ProcessOrderResult
    {
        public bool Processed { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}