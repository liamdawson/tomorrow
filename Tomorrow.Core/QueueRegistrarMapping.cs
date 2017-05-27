using System;

namespace Tomorrow.Core
{
    public class QueueRegistrarMapping
    {
        public Type RegistrarType { get; set; }
        public int HandlerInstances { get; set; } = 0;
    }
}
