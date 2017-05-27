using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tomorrow.Core.Abstractions;

namespace Tomorrow.Core
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class Scheduler : IScheduler
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public const string DefaultQueueName = "default";

        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<TomorrowConfig> _config;

        public Scheduler(IServiceProvider provider, IOptions<TomorrowConfig> config)
        {
            _serviceProvider = provider;
            _config = config;

            var registrations = new List<Task>();

            foreach (var queuePair in _config.Value.Queues)
            {
                var registrarType = queuePair.Value.RegistrarType;
                var handlerInstances = queuePair.Value.HandlerInstances;
                var queueName = queuePair.Key;

                registrations.Add(GetQueueRegistrar(registrarType).RegisterQueue(queueName, handlerInstances));
            }

            Task.WaitAll(registrations.ToArray());
        }

        private IQueueRegistrar GetQueueRegistrar(string queueName)
        {
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));

            var registrarType = _config.Value.Queues[queueName].RegistrarType;
            return GetQueueRegistrar(registrarType);
        }

        private IQueueRegistrar GetQueueRegistrar(Type registrarType)
        {
            if (registrarType == null) throw new ArgumentNullException(nameof(registrarType));

            var registrar = _serviceProvider.GetRequiredService(registrarType) as IQueueRegistrar;

            if (registrar == null)
                throw new ArgumentException("Registrar for queue was missing or not of the correct type.",
                    nameof(registrarType));

            return registrar;
        }

        private async Task<IQueueScheduler> GetQueueScheduler(string queueName)
        {
            return await GetQueueRegistrar(queueName).GetSchedulerForQueue(queueName);
        }

        public async Task Schedule(string queueName, IQueuedJob job, TimeSpan delayBy)
        {
            await (await GetQueueScheduler(queueName)).Schedule(queueName, delayBy, job);
        }
    }
}