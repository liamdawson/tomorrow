using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tomorrow.Abstractions;

namespace Tomorrow.InProcess.UnitTest
{
    [TestClass]
    public class ServiceProviderTests
    {
        private interface IDummyService
        {
            bool ReturnTrue();
        }

        private class DummyService : IDummyService
        {
            public bool ReturnTrue() => true;
        }

        [TestMethod]
        public async Task GetsServiceProvider()
        {
            var scheduler = new ServiceCollection()
                .AddTransient<IDummyService, DummyService>()
                .AddTomorrowInProcess(1)
                .BuildServiceProvider()
                .GetRequiredService<IJobScheduler>();
            var flag = false;

            await scheduler.Schedule((sp) => flag = sp.GetRequiredService<IDummyService>().ReturnTrue());

            await Task.Delay(100);

            Assert.IsTrue(flag, "Flag was not set from service provider service.");
        }
    }
}
