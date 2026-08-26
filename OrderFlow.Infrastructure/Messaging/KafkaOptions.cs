namespace OrderFlow.Infrastructure.Messaging;

public sealed class KafkaOptions
{
    public const string SectionName = "Kafka";
    public string BootstrapServers { get; init; } = "localhost:9092";
    public List<KafkaTopicOptions> Topics { get; init; } = [];
}

public sealed class KafkaTopicOptions
{
    public string Name { get; init; } = string.Empty;
    public int NumPartitions { get; init; } = 1;
    public short ReplicationFactor { get; init; } = 1;
}
