using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Tomorrow.Core.Abstractions
{
    public class ActivatedInstanceMethodJob : IQueuedJob
    {
        public MethodInfo Method { get; set; }
        public object[] Parameters { get; set; }

        public ActivatedInstanceMethodJob()
        {
        }

        public ActivatedInstanceMethodJob(MethodInfo method, params object[] parameters)
        {
            Parameters = parameters;
            Method = method;
        }

        public Task<QueuedJobResult> Perform(IServiceProvider serviceProvider)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var instance = Method.IsStatic ? null : serviceProvider.GetRequiredService(Method.DeclaringType);

                    Method.Invoke(instance, Parameters);
                }
                // when invoked method threw an exception, return _that_ exception
                catch (TargetInvocationException ex) when (ex.HResult == -2146232828)
                {
                    return new QueuedJobResult(ex.InnerException);
                }
                catch (Exception ex)
                {
                    return new QueuedJobResult(ex);
                }

                return new QueuedJobResult();
            });
        }

        public string GetDiagnosticDescription() =>
            $"Invoke method {Method.Name} on a DI-sourced instance of type {Method.DeclaringType.FullName} with parameters {string.Join(", ", Parameters.Select(o => o.ToString()))}";
    }
}