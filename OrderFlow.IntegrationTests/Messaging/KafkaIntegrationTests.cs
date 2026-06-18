using Confluent.Kafka;
using Confluent.Kafka.Admin;
using FluentAssertions;
using OrderFlow.IntegrationTests.Fixtures;

namespace OrderFlow.IntegrationTests.Messaging
{
    public class KafkaIntegrationTests : IClassFixture<KafkaFixture>
    {
        private readonly KafkaFixture _fixture;

        public KafkaIntegrationTests(KafkaFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Kafka_Should_Produce_And_Consume_Message()
        {
            var bootstrapServers = _fixture.Container.GetBootstrapAddress();

            var topicName = $"order-created-test-{Guid.NewGuid():N}";
            var key = Guid.NewGuid().ToString();

            var payload = $$"""
            {
                "orderId": "{{Guid.NewGuid()}}",
                "eventType": "OrderCreated"
            }
            """;

            using var adminClient = new AdminClientBuilder(new AdminClientConfig
            {
                BootstrapServers = bootstrapServers
            }).Build();

            await adminClient.CreateTopicsAsync(new[]
            {
                new TopicSpecification
                {
                    Name = topicName,
                    NumPartitions = 1,
                    ReplicationFactor = 1
                }
            });

            await Task.Delay(3000);

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                Acks = Acks.All
            };

            using var producer = new ProducerBuilder<string, string>(producerConfig).Build();

            var deliveryResult = await producer.ProduceAsync(
                topicName,
                new Message<string, string>
                {
                    Key = key,
                    Value = payload
                });

            producer.Flush(TimeSpan.FromSeconds(10));

            deliveryResult.Status.Should().Be(PersistenceStatus.Persisted);
            deliveryResult.Topic.Should().Be(topicName);

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = $"orderflow-tests-{Guid.NewGuid():N}",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

            consumer.Assign(new TopicPartitionOffset(
                topicName,
                new Partition(0),
                Offset.Beginning));

            ConsumeResult<string, string>? consumed = null;

            var timeout = DateTime.UtcNow.AddSeconds(30);

            while (DateTime.UtcNow < timeout)
            {
                var result = consumer.Consume(TimeSpan.FromSeconds(1));

                if (result is null)
                    continue;

                if (result.Message.Key == key)
                {
                    consumed = result;
                    break;
                }
            }

            consumed.Should().NotBeNull();
            consumed!.Message.Key.Should().Be(key);
            consumed.Message.Value.Should().Be(payload);

            consumer.Close();
        }
    }
}