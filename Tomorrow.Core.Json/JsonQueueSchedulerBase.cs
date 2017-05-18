using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Tomorrow.Core.Json.Serialization;

namespace Tomorrow.Core.Json
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public abstract class JsonQueueSchedulerBase : ITomorrowQueueScheduler
    {
        protected abstract Task SaveDehydratedExpression(string queueName, string expression, DateTime activationTime);

        public class JobRepresentation
        {
            public SimpleTypePointer ActivationType { get; set; }
            public SimpleMethodPointer Method { get; set; }
            public KeyValuePair<SimpleTypePointer, object>[] Parameters { get; set; }
        }

        public async Task Schedule(string queueName, Type activationType, MethodInfo methodInfo, TimeSpan delayBy,
            params object[] parameters)
        {
            var representation = new JobRepresentation
            {
                ActivationType = activationType,
                Method = methodInfo,
                Parameters = parameters.Select(p => new KeyValuePair<SimpleTypePointer, object>(p.GetType(), p)).ToArray()
            };

            var exprString = JsonConvert.SerializeObject(representation);

            await SaveDehydratedExpression(queueName, exprString, DateTime.UtcNow.Add(delayBy));
        }

        protected Action<IServiceProvider> RehydrateExpression(string expression)
        {
            var representation = JsonConvert.DeserializeObject<JobRepresentation>(expression);
            var parameters = representation.Parameters.Select(pair => Convert.ChangeType(pair.Value, pair.Key.Type))
                .ToArray();

            return sp =>
            {
                var target = sp.GetRequiredService(representation.ActivationType);

                representation.Method.MethodInfo.Invoke(target, parameters);
            };
        }
    }
}