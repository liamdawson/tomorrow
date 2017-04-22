using System;
using System.Threading.Tasks;

namespace Tomorrow.Abstractions
{
    public abstract class JobScheduler : IJobScheduler
    {
        public virtual Task Schedule(Action action, TimeSpan timeToWait = default(TimeSpan))
        {
            return Schedule(() =>
            {
                action.Invoke();
                return Task.Run(() => { });
            }, timeToWait);
        }

        public virtual Task Schedule(Action<IServiceProvider> action, TimeSpan timeToWait = default(TimeSpan))
        {
            return Schedule((sp) =>
            {
                action.Invoke(sp);
                return Task.Run(() => { });
            }, timeToWait);
        }

        public virtual Task Schedule(Func<Task> action, TimeSpan timeToWait = default(TimeSpan))
        {
            return Schedule((sp) => action(), timeToWait);
        }

        public abstract Task Schedule(Func<IServiceProvider, Task> action, TimeSpan timeToWait = default(TimeSpan));
    }
}
