using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ServiceBusJobs.Jobs
{
    public class JobDispatcher : IJobDispatcher
    {
        private IServiceProvider provider;
        private ILogger<IJobDispatcher> logger;

        public JobDispatcher(IServiceProvider provider,
            ILogger<IJobDispatcher> logger)
        {
            this.provider = provider;
            this.logger = logger;
        }

        public async Task Dispatch(Message message)
        {
            var type = message.UserProperties["Type"];
            if (type as Type == null)
            {
                type = Type.GetType(type as string);
            }

            logger.LogInformation($"Executing job {type.ToString()}");

            var job = (IJob)provider.GetService(type as Type);
            await job.ExecuteAsync(new MessageContainer(message));
        }
    }
}
