using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OrderFlow.Infrastructure.Messaging;

public sealed class KafkaTopicInitializer : IKafkaTopicInitializer
{
    private readonly KafkaOptions _options;
    private readonly ILogger<KafkaTopicInitializer> _logger;

    public KafkaTopicInitializer(IOptions<KafkaOptions> options, ILogger<KafkaTopicInitializer> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var topics = _options.Topics
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => new TopicSpecification
            {
                Name = x.Name,
                NumPartitions = x.NumPartitions,
                ReplicationFactor = x.ReplicationFactor
            })
            .ToArray();

        if (topics.Length == 0)
            throw new InvalidOperationException("Nenhum tópico foi configurado em Kafka:Topics.");

        using var admin = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = _options.BootstrapServers,
            ClientId = "orderflow-topic-initializer"
        }).Build();

        try
        {
            await admin.CreateTopicsAsync(topics);
            _logger.LogInformation("Tópicos Kafka criados: {Topics}", topics.Select(x => x.Name));
        }
        catch (CreateTopicsException ex)
        {
            var failures = ex.Results.Where(x => x.Error.Code != ErrorCode.TopicAlreadyExists).ToArray();
            if (failures.Length > 0)
                throw new InvalidOperationException($"Falha ao criar tópicos Kafka: {string.Join(", ", failures.Select(x => $"{x.Topic} ({x.Error.Reason})"))}", ex);

            _logger.LogInformation("Tópicos Kafka já existem: {Topics}", topics.Select(x => x.Name));
        }
    }
}
