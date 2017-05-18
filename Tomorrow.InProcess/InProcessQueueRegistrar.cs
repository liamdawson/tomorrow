using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tomorrow.Core;

namespace Tomorrow.InProcess
{
    public class InProcessQueueRegistrar : ITomorrowQueueRegistrar
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptions<InProcessQueueRegistrarSettings> _settings;
        private readonly Dictionary<string, ConcurrentQueue<Action<IServiceProvider>>> _queues;

        public InProcessQueueRegistrar(IServiceScopeFactory scopeFactory, IOptions<InProcessQueueRegistrarSettings> settings)
        {
            _scopeFactory = scopeFactory;
            _settings = settings;
            _queues = new Dictionary<string, ConcurrentQueue<Action<IServiceProvider>>>();
        }

        public Task RegisterQueue(string queueName, int handlers)
        {
            _queues[queueName] = new ConcurrentQueue<Action<IServiceProvider>>();
            for (var i = 0; i < handlers; i++)
            {
                Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        Action<IServiceProvider> candidate;
                        while (!_queues[queueName].TryDequeue(out candidate))
                        {
                            await Task.Delay(_settings.Value.RunnerPollPeriod);
                        }

                        using (var scope = _scopeFactory.CreateScope())
                        {
                            candidate(scope.ServiceProvider);
                        }
                    }

                    // ReSharper disable once FunctionNeverReturns
                });
            }

            return Task.Delay(0);
        }

        public Task<ITomorrowQueueScheduler> GetSchedulerForQueue(string queueName)
        {
            return Task.FromResult<ITomorrowQueueScheduler>(new QueueScheduler(_queues[queueName]));
        }

        private class QueueScheduler : Core.Json.JsonQueueSchedulerBase
        {
            private readonly ConcurrentQueue<Action<IServiceProvider>> _queue;

            public QueueScheduler(ConcurrentQueue<Action<IServiceProvider>> queue)
            {
                _queue = queue;
            }

            protected override async Task SaveDehydratedExpression(string queueName, string expression, DateTime activationTime)
            {
                await Task.Delay(Math.Max(0, (DateTime.UtcNow - activationTime).Milliseconds));
                _queue.Enqueue(RehydrateExpression(expression));
            }
        }
    }
}
