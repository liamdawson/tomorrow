using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tomorrow.Core.Abstractions;

namespace Tomorrow.Core
{
    public static class SchedulerExtensions
    {
        public static async Task Schedule(this IScheduler scheduler, IQueuedJob job)
        {
            await Schedule(scheduler, Scheduler.DefaultQueueName, job);
        }

        public static async Task Schedule(this IScheduler scheduler, IQueuedJob job, TimeSpan delayBy)
        {
            await scheduler.Schedule(Scheduler.DefaultQueueName, job, delayBy);
        }

        public static async Task Schedule(this IScheduler scheduler, string queueName, IQueuedJob job)
        {
            await scheduler.Schedule(queueName, job, TimeSpan.Zero);
        }

        public static async Task Schedule<T>(this IScheduler scheduler, string queueName,
            Expression<Action<T>> expression, TimeSpan delayBy)
        {
            var body = expression.Body as MethodCallExpression;

            if (body == null || body.Method.IsStatic)
            {
                throw new ArgumentException("Only non-static method invocation calls are supported for scheduling an expression.", nameof(expression));
            }
            
            var newJob = new ActivatedInstanceMethodJob(body.Method, expression.Parameters.Select(pe => Expression.Lambda(pe).Compile().DynamicInvoke()));

            await scheduler.Schedule(queueName, newJob, delayBy);
        }

        public static async Task Schedule<T>(this IScheduler scheduler, string queueName,
            Expression<Action<T>> expression) => await Schedule(scheduler, queueName, expression, TimeSpan.Zero);

        public static async Task Schedule<T>(this IScheduler scheduler,
            Expression<Action<T>> expression, TimeSpan delayBy) => await Schedule(scheduler, Scheduler.DefaultQueueName, expression, delayBy);

        public static async Task Schedule<T>(this IScheduler scheduler, 
            Expression<Action<T>> expression) => await Schedule(scheduler, Scheduler.DefaultQueueName, expression, TimeSpan.Zero);
    }
}
