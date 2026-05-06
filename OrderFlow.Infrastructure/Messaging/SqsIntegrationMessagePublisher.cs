using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using OrderFlow.Application.Interfaces;

namespace OrderFlow.Infrastructure.Messaging
{
    public class SqsIntegrationMessagePublisher : IIntegrationMessagePublisher
    {
        private readonly SqsSettings _settings;

        public SqsIntegrationMessagePublisher(SqsSettings settings)
        {
            _settings = settings;
        }

        public async Task PublishAsync(
            string messageType,
            string payload,
            string correlationId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_settings.QueueUrl))
                throw new InvalidOperationException("QueueUrl do SQS não configurada.");

            var region = RegionEndpoint.GetBySystemName(_settings.Region);

            using var client = new AmazonSQSClient(region);

            var request = new SendMessageRequest
            {
                QueueUrl = _settings.QueueUrl,
                MessageBody = payload,
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    ["MessageType"] = new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = messageType
                    },
                    ["CorrelationId"] = new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = correlationId
                    }
                }
            };

            await client.SendMessageAsync(request, cancellationToken);
        }
    }
}