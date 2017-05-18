using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Tomorrow.Core
{
    public interface ITomorrowQueueScheduler
    {
        Task Schedule(string queueName, Type activatedType, MethodInfo methodInfo, TimeSpan delayBy, params object[] parameters);
    }
}
