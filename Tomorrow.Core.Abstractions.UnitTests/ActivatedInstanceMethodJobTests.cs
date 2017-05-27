using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tomorrow.Core.Abstractions.UnitTests
{
    [TestClass]
    public class ActivatedInstanceMethodJobTests
    {
        private class TestException : Exception
        {
        }

        private class TestService
        {
            public bool FlagThrown { get; protected set; } = false;
            public void ThrowFlag() => FlagThrown = true;
            public void ThrowException() => throw new TestException();
            public static void ThrowStaticException() => throw new TestException();
        }

        private static IServiceProvider MakePipeline() => new ServiceCollection().AddSingleton<TestService>().BuildServiceProvider();

        [TestMethod]
        public async Task Executes()
        {
            var pipeline = MakePipeline();
            Action del = new TestService().ThrowFlag;

            var testee = new ActivatedInstanceMethodJob(del.GetMethodInfo());
            var verifier = pipeline.GetRequiredService<TestService>();

            var result = await testee.Perform(pipeline);

            Assert.AreEqual(QueuedJobResult.JobResult.Succeeded, result.Result);
            Assert.AreEqual(true, verifier.FlagThrown);
        }

        [TestMethod]
        public async Task ReturnsErrorResult()
        {
            
            var pipeline = MakePipeline();
            Action del = new TestService().ThrowException;

            var testee = new ActivatedInstanceMethodJob(del.GetMethodInfo());

            var result = await testee.Perform(pipeline);

            Assert.AreEqual(QueuedJobResult.JobResult.Errored, result.Result);
            Assert.IsInstanceOfType(result.Exception, typeof(TestException));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StaticMethodTargetThrowsException()
        {
            Action del = TestService.ThrowStaticException;

            var testee = new ActivatedInstanceMethodJob(del.GetMethodInfo());
        }
    }
}