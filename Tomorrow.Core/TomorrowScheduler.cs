using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Tomorrow.Core
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class TomorrowScheduler : ITomorrowScheduler
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public const string DefaultQueueName = "default";

        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<TomorrowConfig> _config;

        public TomorrowScheduler(IServiceProvider provider, IOptions<TomorrowConfig> config)
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

        private ITomorrowQueueRegistrar GetQueueRegistrar(string queueName)
        {
            if (queueName == null) throw new ArgumentNullException(nameof(queueName));

            var registrarType = _config.Value.Queues[queueName].RegistrarType;
            return GetQueueRegistrar(registrarType);
        }

        private ITomorrowQueueRegistrar GetQueueRegistrar(Type registrarType)
        {
            if (registrarType == null) throw new ArgumentNullException(nameof(registrarType));

            var registrar = _serviceProvider.GetRequiredService(registrarType) as ITomorrowQueueRegistrar;

            if (registrar == null)
                throw new ArgumentException("Registrar for queue was missing or not of the correct type.",
                    nameof(registrarType));

            return registrar;
        }

        private async Task<ITomorrowQueueScheduler> GetQueueScheduler(string queueName)
        {
            return await GetQueueRegistrar(queueName).GetSchedulerForQueue(queueName);
        }

        public async Task Schedule(MethodInfo method, params object[] parameters)
        {
            await Schedule(DefaultQueueName, method, parameters);
        }

        public async Task Schedule(string queueName, MethodInfo method, params object[] parameters)
        {
            await (await GetQueueScheduler(queueName)).Schedule(queueName, method.DeclaringType, method, TimeSpan.Zero, parameters);
        }

        public async Task Schedule<T>(Expression<Action<T>> simpleExpression)
        {
            await Schedule(DefaultQueueName, simpleExpression);
        }

        public async Task Schedule<T>(string queueName, Expression<Action<T>> simpleExpression)
        {
            // only very simple expressions are currently supported

            // assume the call is a singular invocation

            var invocationExpression = simpleExpression.Body as MethodCallExpression;

            if (invocationExpression == null)
            {
                throw new ArgumentException("Only method call invocations are currently allowed as scheduled job expressions.", nameof(simpleExpression));
            }

            var methodInfo = invocationExpression.Method;
            var parameters = invocationExpression.Arguments.Select(expr => Expression.Lambda(expr).Compile().DynamicInvoke()).ToArray();

            await Schedule(queueName, methodInfo, parameters);
        }
    }
}