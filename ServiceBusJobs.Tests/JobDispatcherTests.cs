using Microsoft.Extensions.Logging;
using Moq;
using System;
using JobSystem.Jobs;
using Xunit;
using JobSystem.Queue;

namespace TourMaster.Jobs.Tests
{
    public class JobDispatcherTests
    {
        [Fact]
        public async void JobDispatcher_Dispatch_UsesRegisteredJob()
        {
            var mockJob = new Mock<Job>();
            mockJob.Setup(m => m.ExecuteAsync(It.IsAny<QueueMessage>()))
                .ReturnsAsync(true)
                .Verifiable();

            var mockJobRegistry = new Mock<IJobRegistry>();
            mockJobRegistry.Setup(m => m.Lookup(It.IsAny<Type>()))
                .Returns(mockJob.Object);

            var mockLogService = new Mock<ILogger<JobDispatcher>>();
            mockLogService.Setup(m => m.BeginScope(It.IsAny<QueueMessage>()))
                .Returns(new Disposable());

            var scheduler = new JobDispatcher(mockJobRegistry.Object, mockLogService.Object);

            var message = new QueueMessage(mockJob.Object.GetType().AssemblyQualifiedName);
            message.SetBody("test");

            await scheduler.Dispatch(message);

            mockJob.Verify(m => m.ExecuteAsync(It.Is<QueueMessage>(q => q.GetBody<string>() == "test")), Times.Once);
        }

        private class Disposable : IDisposable
        {
            public void Dispose() { }
        }
    }
}
