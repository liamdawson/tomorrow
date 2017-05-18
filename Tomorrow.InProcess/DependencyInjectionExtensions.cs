using Microsoft.Extensions.DependencyInjection;
using Tomorrow.Core;

namespace Tomorrow.InProcess
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddTomorrowInProcess(this IServiceCollection services)
        {
            return services
                .AddOptions()
                .AddSingleton<InProcessQueueRegistrar>();
        }
    }
}
