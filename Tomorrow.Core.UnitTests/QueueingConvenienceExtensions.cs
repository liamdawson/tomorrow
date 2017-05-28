using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tomorrow.Core.Abstractions;

namespace Tomorrow.Core.UnitTests
{
    [TestClass]
    public class QueueingConvenienceExtensions
    {
        private class TestClass
        {
            public void Nop()
            {
            }


            public void Nop(int i)
            {
            }
        }

    [TestMethod]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public async Task UsesDefaultQueue()
        {
            var mock = new Mock<IScheduler>(MockBehavior.Loose);
            var job = new Mock<IQueuedJob>().Object;
            var target = mock.Object;

            await target.Schedule(job);
            await target.Schedule(job, TimeSpan.FromSeconds(1));
            await target.Schedule<string>(s => s.ToLower());
            await target.Schedule<string>(s => s.ToLower(), TimeSpan.FromSeconds(1));

            mock.Verify(
                scheduler => scheduler.Schedule(Scheduler.DefaultQueueName,
                    It.IsAny<IQueuedJob>(),
                    It.IsAny<TimeSpan>()), Times.Exactly(4));
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public async Task UsesDefaultDelay()
        {
            var mock = new Mock<IScheduler>(MockBehavior.Loose);
            var job = new Mock<IQueuedJob>().Object;
            var target = mock.Object;

            await target.Schedule(job);
            await target.Schedule("queue", job);
            await target.Schedule<string>(s => s.ToLower());
            await target.Schedule<string>("queue", s => s.ToLower());

            mock.Verify(
                scheduler => scheduler.Schedule(It.IsAny<string>(),
                    It.IsAny<IQueuedJob>(),
                    TimeSpan.Zero), Times.Exactly(4));
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public async Task ExpressionFormCreatesCorrectJobWithNoParameters()
        {
            var mock = new Mock<IScheduler>(MockBehavior.Loose);
            var target = mock.Object;
            Action method = new TestClass().Nop;

            await target.Schedule<TestClass>(t => t.Nop());
            await target.Schedule<TestClass>("queue", t => t.Nop());

            mock.Verify(
                scheduler => scheduler.Schedule(It.IsAny<string>(),
                    Match.Create<ActivatedInstanceMethodJob>(job => Equals(job.Method, method.GetMethodInfo())),
                    It.IsAny<TimeSpan>()), Times.Exactly(2));
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public async Task ExpressionFormCreatesJobWithParameters()
        {
            var mock = new Mock<IScheduler>(MockBehavior.Loose);
            var target = mock.Object;

            await target.Schedule<TestClass>(t => t.Nop(3));
            await target.Schedule<TestClass>("queue", t => t.Nop(3));

            mock.Verify(
                scheduler => scheduler.Schedule(It.IsAny<string>(),
                    Match.Create<ActivatedInstanceMethodJob>(job => (int) job.Parameters.Single() == 3),
                    It.IsAny<TimeSpan>()), Times.Exactly(2));
        }
    }
}