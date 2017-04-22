using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tomorrow.InProcess
{
    class InProcessJobScheduler : Abstractions.JobScheduler
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly InProcessJobSchedulerConfig _config;
        private readonly SemaphoreSlim _semaphore;

        public InProcessJobScheduler(IServiceScopeFactory scopeFactory, InProcessJobSchedulerConfig config)
        {
            _scopeFactory = scopeFactory;
            _config = config;
            _semaphore = new SemaphoreSlim(config.MaximumConcurrentJobs, config.MaximumConcurrentJobs);
        }

        public override Task Schedule(Func<IServiceProvider, Task> action, TimeSpan fromNow = default(TimeSpan))
        {
            Task.Delay(fromNow)
                .ContinueWith(async _ =>
                {
                    try
                    {
                        await _semaphore.WaitAsync();

                        using (var scope = _scopeFactory.CreateScope())
                        {
                            await action(scope.ServiceProvider);
                        }
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                });

            return Task.Run(() => { });
        }
    }
}
