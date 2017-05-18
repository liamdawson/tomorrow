using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;

namespace Tomorrow.Core.Json.UnitTest
{
    [TestClass]
    public class QueueSchedulerBaseTests
    {
        private class FlagService
        {
            public bool FlagThrown { get; private set; } = false;
            public int Value { get; private set; } = 0;

            public void ThrowFlag() => FlagThrown = true;
            public void SetValue(int val) => Value = val;
        }

        private class SampleImplementation : JsonQueueSchedulerBase
        {
            public string ProvidedQueueName { get; set; }
            public string Expression { get; set; }
            public DateTime ActivationTime { get; set; }

            protected override Task SaveDehydratedExpression(string queueName, string expression, DateTime activationTime)
            {
                Expression = expression;
                ActivationTime = activationTime;
                ProvidedQueueName = queueName;

                return Task.Delay(0);
            }

            public void RunExpression(IServiceProvider serviceProvider, string expression)
            {
                RehydrateExpression(expression).Invoke(serviceProvider);
            }
        }

        [TestMethod]
        public async Task CanRunScheduledTask()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<FlagService>()
                .BuildServiceProvider();

            var flagService = serviceProvider.GetRequiredService<FlagService>();

            var method = ((Action) flagService.ThrowFlag).GetMethodInfo();

            var testee = new SampleImplementation();

            await testee.Schedule("test", typeof(FlagService), method, TimeSpan.Zero);
            testee.RunExpression(serviceProvider, testee.Expression);

            Assert.IsTrue(flagService.FlagThrown);
        }

        [TestMethod]
        public async Task CanRunScheduledTaskWithParameters()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<FlagService>()
                .BuildServiceProvider();

            var flagService = serviceProvider.GetRequiredService<FlagService>();

            var method = ((Action<int>) flagService.SetValue).GetMethodInfo();

            var testee = new SampleImplementation();

            await testee.Schedule("test", typeof(FlagService), method, TimeSpan.Zero, 42);
            testee.RunExpression(serviceProvider, testee.Expression);

            Assert.AreEqual(42, flagService.Value);
        }
    }
}
