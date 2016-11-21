using Moq;
using System;
using System.Threading.Tasks;
using JobSystem.Jobs;
using Xunit;
using JobSystem.Queue;

namespace TourMaster.Jobs.Tests
{
    public class JobSchedulerTests
    {
        [Fact]
        public async void JobScheduler_Schedule_ThrowsForUnregisteredJob()
        {
            var mockJob = new Mock<Job>();

            var mockJobRegistry = new Mock<IJobRegistry>();
            mockJobRegistry.Setup(m => m.Lookup(It.IsAny<Type>()))
                .Returns<Job>(null);

            var scheduler = new JobScheduler(null, mockJobRegistry.Object);
            
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(async () => await scheduler.Schedule(mockJob.Object.GetType(), "test"));
        }

        [Fact]
        public async void JobScheduler_Schedule_CallsQueueClient()
        {
            var mockJob = new Mock<Job>();

            var mockQueueClient = new Mock<IQueueSender>();
            mockQueueClient.Setup(m => m.SendAsync(It.IsAny<QueueMessage>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var mockJobRegistry = new Mock<IJobRegistry>();
            mockJobRegistry.Setup(m => m.Lookup(It.IsAny<Type>()))
                .Returns(mockJob.Object);

            var scheduler = new JobScheduler(mockQueueClient.Object, mockJobRegistry.Object);

            await scheduler.Schedule(mockJob.Object.GetType(), "test");

            mockQueueClient.Verify(m => m.SendAsync(It.Is<QueueMessage>(q => q.Topic == mockJob.Object.GetType().AssemblyQualifiedName)), Times.Once);
            mockQueueClient.Verify(m => m.SendAsync(It.Is<QueueMessage>(q => q.GetBody<string>() == "test")), Times.Once);
        }
    }
}
