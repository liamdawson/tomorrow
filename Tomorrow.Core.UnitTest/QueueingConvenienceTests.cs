using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tomorrow.Core.UnitTest
{
    [TestClass]
    public class QueueingConvenienceTests
    {
        private class InspectableQueueScheduler : ITomorrowQueueScheduler
        {
            public string QueueName { get; set; }
            public MethodInfo MethodInfo { get; set; }
            public TimeSpan DelayBy { get; set; }
            public object[] Parameters { get; set; }
            public Type ActivatedType { get; set; }

            public Task Schedule(string queueName, Type activatedType, MethodInfo methodInfo, TimeSpan delayBy,
                params object[] parameters)
            {
                QueueName = queueName;
                MethodInfo = methodInfo;
                DelayBy = delayBy;
                Parameters = parameters;
                ActivatedType = activatedType;

                return Task.Delay(0);
            }
        }

        private class InspectableQueueRegistrar : ITomorrowQueueRegistrar
        {
            public Task RegisterQueue(string queueName, int handlers) => Task.Delay(0);
            public InspectableQueueScheduler Scheduler { get; set; } = new InspectableQueueScheduler();

            public Task<ITomorrowQueueScheduler> GetSchedulerForQueue(string queueName) => Task.FromResult(
                (ITomorrowQueueScheduler) Scheduler);
        }

        private class TestService
        {
            public void SimpleAction()
            {
            }

            public void ParameterizedAction(int i)
            {
            }
        }

        [TestMethod]
        public async Task CanQueueASimpleInvocation()
        {
            var sp = new ServiceCollection()
                .AddSingleton<TestService>()
                .AddSingleton<InspectableQueueRegistrar>()
                .AddTomorrow(config =>
                {
                    config.RegisterQueues<InspectableQueueRegistrar>(TomorrowScheduler.DefaultQueueName);
                })
                .BuildServiceProvider();

            var scheduler = sp.GetRequiredService<ITomorrowScheduler>();
            var queueScheduler = sp.GetRequiredService<InspectableQueueRegistrar>().Scheduler;

            await scheduler.Schedule<TestService>(ts => ts.SimpleAction());

            Assert.AreEqual(typeof(TestService).GetTypeInfo().GetMethod(nameof(TestService.SimpleAction)),
                queueScheduler.MethodInfo);

            Assert.AreEqual(TomorrowScheduler.DefaultQueueName, queueScheduler.QueueName);
            Assert.AreEqual(0, queueScheduler.Parameters.Length);
            Assert.AreEqual(TimeSpan.Zero, queueScheduler.DelayBy);
            Assert.AreEqual(typeof(TestService), queueScheduler.ActivatedType);
        }

        [TestMethod]
        public async Task CanQueueAParameterizedInvocation()
        {
            var sp = new ServiceCollection()
                .AddSingleton<TestService>()
                .AddSingleton<InspectableQueueRegistrar>()
                .AddTomorrow(config =>
                {
                    config.RegisterQueues<InspectableQueueRegistrar>(TomorrowScheduler.DefaultQueueName);
                })
                .BuildServiceProvider();

            var scheduler = sp.GetRequiredService<ITomorrowScheduler>();
            var queueScheduler = sp.GetRequiredService<InspectableQueueRegistrar>().Scheduler;

            await scheduler.Schedule<TestService>(ts => ts.ParameterizedAction(42));

            Assert.AreEqual(typeof(TestService).GetTypeInfo().GetMethod(nameof(TestService.ParameterizedAction)),
                queueScheduler.MethodInfo);

            Assert.AreEqual(TomorrowScheduler.DefaultQueueName, queueScheduler.QueueName);
            Assert.AreEqual(1, queueScheduler.Parameters.Length);
            Assert.AreEqual(42, (int)queueScheduler.Parameters[0]);
            Assert.AreEqual(TimeSpan.Zero, queueScheduler.DelayBy);
            Assert.AreEqual(typeof(TestService), queueScheduler.ActivatedType);
        }

        [TestMethod]
        public async Task CanQueueAParameterizedInvocationWithANonConstantValue()
        {
            var valBuffer = new byte[1];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(valBuffer);
            var testVal = (int)valBuffer[0];
            Func<int> valueGetter = () => testVal;

            var sp = new ServiceCollection()
                .AddSingleton<TestService>()
                .AddSingleton<InspectableQueueRegistrar>()
                .AddTomorrow(config =>
                {
                    config.RegisterQueues<InspectableQueueRegistrar>(TomorrowScheduler.DefaultQueueName);
                })
                .BuildServiceProvider();

            var scheduler = sp.GetRequiredService<ITomorrowScheduler>();
            var queueScheduler = sp.GetRequiredService<InspectableQueueRegistrar>().Scheduler;

            await scheduler.Schedule<TestService>(ts => ts.ParameterizedAction(valueGetter()));

            Assert.AreEqual(typeof(TestService).GetTypeInfo().GetMethod(nameof(TestService.ParameterizedAction)),
                queueScheduler.MethodInfo);

            Assert.AreEqual(TomorrowScheduler.DefaultQueueName, queueScheduler.QueueName);
            Assert.AreEqual(1, queueScheduler.Parameters.Length);
            Assert.AreEqual(testVal, (int)queueScheduler.Parameters[0]);
            Assert.AreEqual(TimeSpan.Zero, queueScheduler.DelayBy);
            Assert.AreEqual(typeof(TestService), queueScheduler.ActivatedType);
        }
    }
}