using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tomorrow.Core;

namespace Tomorrow.InProcess.UnitTest
{
    [TestClass]
    public class BasicChecks
    {
        internal class TestService
        {
            public void ThrowFlag()
            {
                FlagThrown = true;
            }
            public bool FlagThrown { get; private set; }
        }

        [TestMethod]
        public async Task QueueWithRunnersRunsScheduledJob()
        {
            var services = new ServiceCollection()
                .AddSingleton<TestService>()
                .Configure<InProcessQueueRegistrarSettings>(settings => settings.RunnerPollPeriod = TimeSpan.FromMilliseconds(1))
                .AddTomorrowInProcess()
                .AddTomorrow(config =>
                {
                    config.RegisterQueues<InProcessQueueRegistrar>(1, TomorrowScheduler.DefaultQueueName);
                })
                .BuildServiceProvider();

            var flagService = services.GetRequiredService<TestService>();
            var scheduler = services.GetRequiredService<ITomorrowScheduler>();

            await scheduler.Schedule(typeof(TestService).GetTypeInfo()
                .GetDeclaredMethod(nameof(TestService.ThrowFlag)));

            await Task.Delay(50);

            Assert.IsTrue(flagService.FlagThrown);
        }

        [TestMethod]
        public async Task QueueWithNoRunnersDoesNotRunScheduledJob()
        {
            var services = new ServiceCollection()
                .AddSingleton<TestService>()
                .Configure<InProcessQueueRegistrarSettings>(settings => settings.RunnerPollPeriod = TimeSpan.FromMilliseconds(1))
                .AddTomorrowInProcess()
                .AddTomorrow(config =>
                {
                    config.RegisterQueues<InProcessQueueRegistrar>(TomorrowScheduler.DefaultQueueName);
                })
                .BuildServiceProvider();

            var flagService = services.GetRequiredService<TestService>();
            var scheduler = services.GetRequiredService<ITomorrowScheduler>();

            await scheduler.Schedule(typeof(TestService).GetTypeInfo()
                .GetDeclaredMethod(nameof(TestService.ThrowFlag)));

            await Task.Delay(50);

            Assert.IsFalse(flagService.FlagThrown);
        }
    }
}
