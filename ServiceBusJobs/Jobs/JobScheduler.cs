using JobSystem.Queue;
using System;
using System.Threading.Tasks;

namespace JobSystem.Jobs
{
    public class JobScheduler : IJobScheduler
    {
        private readonly IQueueSender queueClient;
        private readonly IJobRegistry jobRegistry;

        public JobScheduler(IQueueSender queueClient,
            IJobRegistry jobRegistry)
        {
            this.queueClient = queueClient;
            this.jobRegistry = jobRegistry;
        }

        public async Task Schedule<T>(Type type, T body)
        {
            var job = jobRegistry.Lookup(type);
            if(job == null)
            {
                throw new ArgumentException($"Type '{type.AssemblyQualifiedName}' not found in service descriptor.");
            }

            var message = new QueueMessage(type.AssemblyQualifiedName);
            message.SetBody(body);

            await queueClient.SendAsync(message);
        }
    }
}
