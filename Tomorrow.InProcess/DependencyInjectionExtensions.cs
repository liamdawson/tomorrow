using Microsoft.Extensions.DependencyInjection;

namespace Tomorrow.InProcess
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddTomorrowInProcess(this IServiceCollection services)
        {
            return services
                .AddSingleton<InProcessQueueRegistrar>();
        }
    }
}
