namespace OrderFlow.Infrastructure.Messaging
{
    public class SqsSettings
    {
        public bool Enabled { get; set; }
        public string Region { get; set; } = "sa-east-1";
        public string QueueUrl { get; set; } = string.Empty;
        public int MaxMessages { get; set; } = 5;
        public int WaitTimeSeconds { get; set; } = 10;
    }
}