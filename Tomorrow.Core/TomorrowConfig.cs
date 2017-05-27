using System.Collections.Generic;

namespace Tomorrow.Core
{
    public class TomorrowConfig
    {
        public Dictionary<string, QueueRegistrarMapping> Queues { get; set; } = new Dictionary<string, QueueRegistrarMapping>();
    }
}
