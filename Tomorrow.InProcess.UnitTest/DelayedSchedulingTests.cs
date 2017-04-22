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
            int flag = 0;
            var scheduler = TestHelper.MakeInstance();

            await scheduler.Schedule(() => flag++, TimeSpan.FromSeconds(0.2));
            await scheduler.Schedule((sp) => flag++, TimeSpan.FromSeconds(0.2));
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await scheduler.Schedule(async () => flag++, TimeSpan.FromSeconds(0.2));
            await scheduler.Schedule(async () => flag++, TimeSpan.FromSeconds(0.2));
#pragma warning restore CS1998

            Assert.AreEqual(0, flag, "Scheduler fired tasks before the delay finished.");

            await Task.Delay(TimeSpan.FromSeconds(0.5));

            Assert.AreEqual(4, flag, "Scheduler did not fire all tasks in time.");
        }
    }
}
