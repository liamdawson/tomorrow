using System.Threading.Tasks;

namespace Tomorrow.Core
{
    public interface ITomorrowQueueRegistrar
    {
        Task RegisterQueue(string queueName, int handlers);
        Task<ITomorrowQueueScheduler> GetSchedulerForQueue(string queueName);
    }
}
