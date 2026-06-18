using Confluent.Kafka;
using FluentAssertions;

namespace OrderFlow.IntegrationTests.Messaging
{
    public class KafkaIntegrationTests
    {
        private const string BootstrapServers = "localhost:9092";
        private const string TopicName = "order-created-test";

        [Fact]
        public async Task Kafka_Should_Produce_And_Consume_Message()
        {
            // Arrange
            var key = Guid.NewGuid().ToString();
            var payload = $$"""
            {
                "orderId": "{{Guid.NewGuid()}}",
                "eventType": "OrderCreated"
            }
            """;

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = BootstrapServers
            };

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = BootstrapServers,
                GroupId = $"orderflow-tests-{Guid.NewGuid()}",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var producer = new ProducerBuilder<string, string>(producerConfig).Build();

            using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

            consumer.Subscribe(TopicName);

            // Act
            var deliveryResult = await producer.ProduceAsync(
                TopicName,
                new Message<string, string>
                {
                    Key = key,
                    Value = payload
                });

            producer.Flush(TimeSpan.FromSeconds(10));

            // Assert producer
            deliveryResult.Status.Should().Be(PersistenceStatus.Persisted);
            deliveryResult.Topic.Should().Be(TopicName);

            // Assert consumer
            ConsumeResult<string, string>? consumed = null;

            var timeout = DateTime.UtcNow.AddSeconds(20);

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
            consumed!.Message.Value.Should().Be(payload);
            consumed.Message.Key.Should().Be(key);

            consumer.Close();
        }
    }
}