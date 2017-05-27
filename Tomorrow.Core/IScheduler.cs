using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Tomorrow.Core.Abstractions;

namespace Tomorrow.Core
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface IScheduler
    {
        Task Schedule(string queueName, IQueuedJob job, TimeSpan delayBy);
    }
}