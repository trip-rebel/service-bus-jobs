using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;

namespace ServiceBusJobs.Jobs
{
    public interface IJob
    {
        Task<bool> ExecuteAsync(IMessageContainer message);
    }
}