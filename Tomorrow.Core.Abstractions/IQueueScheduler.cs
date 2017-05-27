using System;
using System.Threading.Tasks;

namespace Tomorrow.Core.Abstractions
{
    public interface IQueueScheduler
    {
        Task Schedule(string queueName, TimeSpan delayBy, IQueuedJob queuedJob);
    }
}
