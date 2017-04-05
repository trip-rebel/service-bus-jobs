using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusJobs.Jobs
{
    public class JobScheduler : IJobScheduler
    {
        private QueueClient queueClient;

        public JobScheduler(IOptions<JobSystemOptions> optionsAccessor)
        {
            queueClient = new QueueClient(optionsAccessor.Value.ServiceBusConnectionString,
                optionsAccessor.Value.ServiceBusName,
                ReceiveMode.PeekLock);
        }

        public async Task Schedule<T>(Type type, T body)
        {
            var serializedBody = JsonConvert.SerializeObject(body);
            var message = new Message(Encoding.UTF8.GetBytes(serializedBody));

            message.UserProperties["BodyType"] = body?.GetType()?.AssemblyQualifiedName ?? string.Empty;
            message.UserProperties["Type"] = type.AssemblyQualifiedName;
            
            await queueClient.SendAsync(message);
        }
    }
}
