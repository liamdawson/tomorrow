using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Tomorrow.Abstractions;

namespace Tomorrow.InProcess
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddTomorrowInProcess(this IServiceCollection services, int maximumConcurrentJobs = 5)
        {
            return services
                .AddSingleton(new InProcessJobSchedulerConfig
                {
                    MaximumConcurrentJobs = maximumConcurrentJobs
                })
                .AddSingleton<IJobScheduler, InProcessJobScheduler>();
        }
    }
}
