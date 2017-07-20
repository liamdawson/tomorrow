using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tomorrow.Core.Abstractions.UnitTests
{
    public class ActivatedInstanceMethodJobTests
    {
        private class TestException : Exception
        {
        }

        private class TestService
        {
            public bool FlagThrown { get; private set; } = false;
            public void ThrowFlag() => FlagThrown = true;
            public void ThrowException() => throw new TestException();
            public static void ThrowStaticException() => throw new TestException();
        }

        private static IServiceProvider MakePipeline() => new ServiceCollection().AddSingleton<TestService>().BuildServiceProvider();

        [Fact]
        public async Task Executes()
        {
            var pipeline = MakePipeline();
            Action del = new TestService().ThrowFlag;

            var testee = new ActivatedInstanceMethodJob(del.GetMethodInfo());
            var verifier = pipeline.GetRequiredService<TestService>();

            var result = await testee.Perform(pipeline);

            Assert.Equal(QueuedJobResult.JobResult.Succeeded, result.Result);
            Assert.True(verifier.FlagThrown);
        }

        [Fact]
        public async Task ReturnsErrorResult()
        {
            
            var pipeline = MakePipeline();
            Action del = new TestService().ThrowException;

            var testee = new ActivatedInstanceMethodJob(del.GetMethodInfo());

            var result = await testee.Perform(pipeline);

            Assert.Equal(QueuedJobResult.JobResult.Errored, result.Result);
            Assert.IsType<TestException>(result.Exception);
        }

        [Fact]
        public void StaticMethodTargetThrowsException()
        {
            Action del = TestService.ThrowStaticException;

            Assert.Throws<ArgumentException>(() =>
            {
                var unused = new ActivatedInstanceMethodJob(del.GetMethodInfo());
            });
        }
    }
}