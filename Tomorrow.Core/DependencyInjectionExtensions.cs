using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Tomorrow.Core;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class TomorrowDependencyInjectionExtensions
    {
        public static IServiceCollection AddTomorrow(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddOptions()
                .Configure<TomorrowConfig>(configuration)
                .AddScoped<ITomorrowScheduler, TomorrowScheduler>();
        }

        public static IServiceCollection AddTomorrow(this IServiceCollection services, Action<TomorrowConfig> configurationAction)
        {
            return services
                .AddOptions()
                .Configure(configurationAction)
                .AddScoped<ITomorrowScheduler, TomorrowScheduler>();
        }

        public static TomorrowConfig RegisterQueues<T>(this TomorrowConfig config, params string[] queues) where T : ITomorrowQueueRegistrar
        {
            return config.RegisterQueues<T>(0, queues);
        }

        public static TomorrowConfig RegisterQueues<T>(this TomorrowConfig config, int handlerInstances, params string[] queues) where T : ITomorrowQueueRegistrar
        {
            foreach (var queue in queues)
            {
                config.Queues.Add(queue, new TomorrowQueueRegistrarMapping
                {
                    HandlerInstances = handlerInstances,
                    RegistrarType = typeof(T)
                });
            }

            return config;
        }
    }
}
