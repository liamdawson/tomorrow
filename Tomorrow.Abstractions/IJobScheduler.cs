using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tomorrow.Abstractions
{
    public interface IJobScheduler
    {
        /// <summary>
        /// Schedules <paramref name="action"/> to occur after <paramref name="timeToWait"/> has passed.
        /// </summary>
        /// <param name="action">Action to perform after <paramref name="timeToWait"/> passes.</param>
        /// <param name="timeToWait">TimeSpan to wait before performing <paramref name="action"/>. Treated as <see cref="TimeSpan.Zero"/> if not provided.</param>
        /// <returns>Task that signals completion of the scheduling process.</returns>
        Task Schedule(Action action, TimeSpan timeToWait = default(TimeSpan));
        /// <summary>
        /// Schedules <paramref name="action"/> receiving a service provider, to occur after <paramref name="timeToWait"/> has passed.
        /// </summary>
        /// <param name="action">Action that receives a <see cref="IServiceProvider"/> to perform after <paramref name="timeToWait"/> passes.</param>
        /// <param name="timeToWait">TimeSpan to wait before performing <paramref name="action"/>. Treated as <see cref="TimeSpan.Zero"/> if not provided.</param>
        /// <returns>Task that signals completion of the scheduling process.</returns>
        Task Schedule(Action<IServiceProvider> action, TimeSpan timeToWait = default(TimeSpan));
        /// <summary>
        /// Schedules asynchronous <paramref name="action"/> to occur after <paramref name="timeToWait"/> has passed.
        /// </summary>
        /// <param name="action">Asynchronous action to perform after <paramref name="timeToWait"/> passes.</param>
        /// <param name="timeToWait">TimeSpan to wait before performing <paramref name="action"/>. Treated as <see cref="TimeSpan.Zero"/> if not provided.</param>
        /// <returns>Task that signals completion of the scheduling process.</returns>
        Task Schedule(Func<Task> action, TimeSpan timeToWait = default(TimeSpan));
        /// <summary>
        /// Schedules asynchronous <paramref name="action"/> receiving a service provider, to occur after <paramref name="timeToWait"/> has passed.
        /// </summary>
        /// <param name="action">Asynchronous action that receives a <see cref="IServiceProvider"/> to perform after <paramref name="timeToWait"/> passes.</param>
        /// <param name="timeToWait">TimeSpan to wait before performing <paramref name="action"/>. Treated as <see cref="TimeSpan.Zero"/> if not provided.</param>
        /// <returns>Task that signals completion of the scheduling process.</returns>
        Task Schedule(Func<IServiceProvider, Task> action, TimeSpan timeToWait = default(TimeSpan));
    }
}
