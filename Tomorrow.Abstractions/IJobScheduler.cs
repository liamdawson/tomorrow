using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Tomorrow.Abstractions
{
    public interface IJobScheduler
    {
        /// <summary>
        /// Schedules <paramref name="action"/> receiving a service provider, to occur after <paramref name="timeToWait"/> has passed.
        /// </summary>
        /// <param name="action">Action that receives a <see cref="IServiceProvider"/> to perform after <paramref name="timeToWait"/> passes.</param>
        /// <param name="timeToWait">TimeSpan to wait before performing <paramref name="action"/>. Treated as <see cref="TimeSpan.Zero"/> if not provided.</param>
        /// <returns>Task that signals completion of the scheduling process.</returns>
        Task Schedule(Expression<Action<IServiceProvider>> action, TimeSpan timeToWait = default(TimeSpan));
        /// <summary>
        /// Schedules asynchronous <paramref name="action"/> receiving a service provider, to occur after <paramref name="timeToWait"/> has passed.
        /// </summary>
        /// <param name="action">Asynchronous action that receives a <see cref="IServiceProvider"/> to perform after <paramref name="timeToWait"/> passes.</param>
        /// <param name="timeToWait">TimeSpan to wait before performing <paramref name="action"/>. Treated as <see cref="TimeSpan.Zero"/> if not provided.</param>
        /// <returns>Task that signals completion of the scheduling process.</returns>
        Task Schedule(Expression<Func<IServiceProvider, Task>> action, TimeSpan timeToWait = default(TimeSpan));
    }
}
