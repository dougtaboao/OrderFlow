namespace OrderFlow.Infrastructure.Messaging
{
    public class SqsSettings
    {
        public string Region { get; set; } = "sa-east-1";
        public string QueueUrl { get; set; } = string.Empty;
        public bool Enabled { get; set; }
    }
}