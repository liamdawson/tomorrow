using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tomorrow.Core.Abstractions;

namespace Tomorrow.Core.Json
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public abstract class JsonQueueSchedulerBase : IQueueScheduler
    {
        protected JsonSerializer GetJsonSerializer()
        {
            return new JsonSerializer
            {
                Converters = 
                {
                    new StrictTypeJsonConverter(),
                    new MethodInfoJsonConverter()
                },
                TypeNameHandling = TypeNameHandling.Auto
            };
        }

        protected abstract Task SaveDehydratedExpression(string queueName, string expression, DateTime activationTime);

        protected Func<IServiceProvider, Task<QueuedJobResult>> RehydrateExpression(string expression)
        {
            var representation = JsonConvert.DeserializeObject<IQueuedJob>(expression);
            return sp => representation.Perform(sp);
        }

        public async Task Schedule(string queueName, TimeSpan delayBy, IQueuedJob queuedJob)
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            using (var reader = new StreamReader(memoryStream))
            {
                GetJsonSerializer().Serialize(writer, queuedJob, queuedJob.GetType());
                await writer.FlushAsync();
                var expression = await reader.ReadToEndAsync();

                await SaveDehydratedExpression(queueName, expression, DateTime.UtcNow + delayBy);

            }
        }
    }
}