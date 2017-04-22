using Microsoft.Extensions.DependencyInjection;
using Tomorrow.Abstractions;

namespace Tomorrow.InProcess.UnitTest
{
    internal class TestHelper
    {
        internal static IJobScheduler MakeInstance()
        {
            var services = new ServiceCollection();

            services.AddTomorrowInProcess(1);

            return services.BuildServiceProvider().GetRequiredService<IJobScheduler>();
        }
    }
}
