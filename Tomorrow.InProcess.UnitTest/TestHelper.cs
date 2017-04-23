using Microsoft.Extensions.DependencyInjection;
using Tomorrow.Abstractions;

namespace Tomorrow.InProcess.UnitTest
{
    internal class TestHelper
    {
        internal static (IJobScheduler JobScheduler, FlagService FlagService) MakeInstance()
        {
            var services = new ServiceCollection();

            services
                .AddSingleton<FlagService>()
                .AddTomorrowInProcess(1);

            var sp = services.BuildServiceProvider();

            return (
                JobScheduler: sp.GetRequiredService<IJobScheduler>(),
                FlagService: sp.GetRequiredService<FlagService>());
        }
    }
}
