using System;
using System.Collections.Generic;
using System.Text;

namespace Tomorrow.Core
{
    public class TomorrowConfig
    {
        public Dictionary<string, TomorrowQueueRegistrarMapping> Queues { get; set; } = new Dictionary<string, TomorrowQueueRegistrarMapping>();
    }
}
