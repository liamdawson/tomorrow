using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Tomorrow.InProcess.UnitTest
{
    [TestClass]
    public class BasicSchedulingTests
    {
        [TestMethod]
        public async Task SimplestActionJobIsExecuted()
        {
            var flag = false;
            var scheduler = TestHelper.MakeInstance();

            await scheduler.Schedule(() => flag = true);

            await Task.Delay(100);

            Assert.IsTrue(flag, "Task was not executed.");
        }

        [TestMethod]
        public async Task ServiceProviderActionJobIsExecuted()
        {
            var flag = false;
            var scheduler = TestHelper.MakeInstance();

            await scheduler.Schedule((sp) => flag = true);

            await Task.Delay(100);

            Assert.IsTrue(flag, "Task was not executed.");
        }

        [TestMethod]
        public async Task SimplestTaskJobIsExecuted()
        {
            var flag = false;
            var scheduler = TestHelper.MakeInstance();

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await scheduler.Schedule(async () => flag = true);
#pragma warning restore CS1998

            await Task.Delay(100);

            Assert.IsTrue(flag, "Task was not executed.");
        }

        [TestMethod]
        public async Task ServiceProviderTaskJobIsExecuted()
        {
            var flag = false;
            var scheduler = TestHelper.MakeInstance();

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await scheduler.Schedule(async (sp) => flag = true);
#pragma warning restore CS1998

            await Task.Delay(100);

            Assert.IsTrue(flag, "Task was not executed.");
        }
    }
}
