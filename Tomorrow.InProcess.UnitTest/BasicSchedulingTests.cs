using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Tomorrow.InProcess.UnitTest
{
    [TestClass]
    public class BasicSchedulingTests
    {
        [TestMethod]
        public async Task ServiceProviderActionJobIsExecuted()
        {
            var (scheduler, flagger) = TestHelper.MakeInstance();

            await scheduler.Schedule((sp) => sp.GetRequiredService<FlagService>().Call());

            await Task.Delay(100);

            Assert.IsTrue(flagger.Called, "Task was not executed.");
        }

        [TestMethod]
        public async Task ServiceProviderTaskJobIsExecuted()
        {
            var (scheduler, flagger) = TestHelper.MakeInstance();

            await scheduler.Schedule((sp) => Task.Run(() => sp.GetRequiredService<FlagService>().Call()));

            await Task.Delay(100);

            Assert.IsTrue(flagger.Called, "Task was not executed.");
        }
    }
}
