using System;

namespace Tomorrow.InProcess
{
    public class InProcessQueueRegistrarSettings
    {
        public TimeSpan RunnerPollPeriod { get; set; }= TimeSpan.FromSeconds(1);
    }
}
