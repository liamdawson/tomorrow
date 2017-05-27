using System.Threading.Tasks;

namespace Tomorrow.Core.Abstractions
{
    public interface IQueueRegistrar
    {
        Task RegisterQueue(string queueName, int handlers);
        Task<IQueueScheduler> GetSchedulerForQueue(string queueName);
    }
}
