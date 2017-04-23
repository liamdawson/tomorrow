using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;

namespace Tomorrow.Abstractions
{
    public abstract class JobScheduler : IJobScheduler
    {
        public virtual Task Schedule(Expression<Action<IServiceProvider>> action, TimeSpan timeToWait = default(TimeSpan))
        {
            var taskDelayMethod = typeof(Task).GetRuntimeMethod("Delay", new Type[] { typeof(int) });

            var wrapperExpression = Expression.Block(typeof(Task), new Expression[] {
                action.Body,
                Expression.Call(taskDelayMethod, Expression.Constant(0))
            });

            return Schedule(Expression.Lambda<Func<IServiceProvider, Task>>(wrapperExpression, action.Parameters), timeToWait);
        }

        public abstract Task Schedule(Expression<Func<IServiceProvider, Task>> action, TimeSpan timeToWait = default(TimeSpan));
    }
}
