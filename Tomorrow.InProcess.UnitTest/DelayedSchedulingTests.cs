using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tomorrow.InProcess.UnitTest
{
    [TestClass]
    public class DelayedSchedulingTests
    {
        [TestMethod]
        public async Task PointFiveSecondDelay()
        {
            var (scheduler, flagger) = TestHelper.MakeInstance();

            await scheduler.Schedule((sp) => sp.GetRequiredService<FlagService>().Call(), TimeSpan.FromSeconds(0.2));
            await scheduler.Schedule((sp) => Task.Run(() => sp.GetRequiredService<FlagService>().Call()), TimeSpan.FromSeconds(0.2));

            Assert.AreEqual(0, flagger.Calls, "Scheduler fired tasks before the delay finished.");

            await Task.Delay(TimeSpan.FromSeconds(0.5));

            Assert.AreEqual(2, flagger.Calls, "Scheduler did not fire all tasks in time.");
        }
    }
}
