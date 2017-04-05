using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;

namespace ServiceBusJobs.Jobs
{
    public abstract class Job : IJob
    {
        public abstract Task<bool> ExecuteAsync(IMessageContainer message);
    }
}
