using System;
using System.Collections.Generic;
using System.Text;

namespace OrderFlow.Worker
{
    public sealed class WorkerSettings
    {
        public bool EnableOrderConsumer { get; set; } = true;
        public bool EnableOutboxPublisher { get; set; } = true;
        public bool EnableKafkaAudit { get; set; } = true;
    }
}
