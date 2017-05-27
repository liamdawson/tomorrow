using System;
using System.Threading.Tasks;

namespace Tomorrow.Core.Abstractions
{
    public interface IQueuedJob
    {
        Task<QueuedJobResult> Perform(IServiceProvider serviceProvider);
        string GetDiagnosticDescription();
    }
}
