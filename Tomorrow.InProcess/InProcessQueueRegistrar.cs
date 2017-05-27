using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tomorrow.Core.Abstractions;

namespace Tomorrow.InProcess
{
    public class InProcessQueueRegistrar : IQueueRegistrar
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Dictionary<string, ConcurrentQueue<Func<IServiceProvider, Task<QueuedJobResult>>>> _queues;
        private readonly Dictionary<string, SemaphoreSlim> _queueLocks;

        public InProcessQueueRegistrar(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _queues = new Dictionary<string, ConcurrentQueue<Func<IServiceProvider, Task<QueuedJobResult>>>>();
            _queueLocks = new Dictionary<string, SemaphoreSlim>();
        }

        public Task RegisterQueue(string queueName, int handlers)
        {
            _queues[queueName] = new ConcurrentQueue<Func<IServiceProvider, Task<QueuedJobResult>>>();
            _queueLocks[queueName] = new SemaphoreSlim(0);

            for (var i = 0; i < handlers; i++)
            {
                Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        await _queueLocks[queueName].WaitAsync();

                        Func<IServiceProvider, Task<QueuedJobResult>> candidate;

                        if (_queues[queueName].TryDequeue(out candidate))
                        {
                            using (var scope = _scopeFactory.CreateScope())
                            {
                                await candidate(scope.ServiceProvider);
                            }
                        }
                        else
                        {
                            _queueLocks[queueName].Release();
                        }
                    }

                    // ReSharper disable once FunctionNeverReturns
                });
            }

            return Task.Delay(0);
        }

        public Task<IQueueScheduler> GetSchedulerForQueue(string queueName)
        {
            return Task.FromResult<IQueueScheduler>(new QueueScheduler(_queues[queueName], _queueLocks[queueName]));
        }

        private class QueueScheduler : Core.Json.JsonQueueSchedulerBase
        {
            private readonly ConcurrentQueue<Func<IServiceProvider, Task<QueuedJobResult>>> _queue;
            private readonly SemaphoreSlim _semaphore;

            public QueueScheduler(ConcurrentQueue<Func<IServiceProvider, Task<QueuedJobResult>>> queue, SemaphoreSlim semaphore)
            {
                _queue = queue;
                _semaphore = semaphore;
            }

            protected override Task SaveDehydratedExpression(string queueName, string expression,
                DateTime activationTime)
            {
                Task.Delay(Math.Max(0, (DateTime.UtcNow - activationTime).Milliseconds))
                    .ContinueWith(_ =>
                    {
                        _queue.Enqueue(RehydrateExpression(expression));
                        _semaphore.Release();
                    }).GetAwaiter();

                return Task.Delay(0);
            }
        }
    }
}