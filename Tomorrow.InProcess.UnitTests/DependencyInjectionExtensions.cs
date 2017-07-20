using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tomorrow.InProcess.UnitTests
{
    public class DependencyInjectionExtensions
    {
        [Fact]
        public void AddTomorrowInProcessAddsRegistrar()
        {
            var pipeline = new ServiceCollection();
            
            pipeline.AddTomorrowInProcess();

            var target = pipeline.BuildServiceProvider();
            
            Assert.NotNull(target.GetService<InProcessQueueRegistrar>());
        }
    }
}