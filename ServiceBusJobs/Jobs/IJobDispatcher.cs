using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;

namespace ServiceBusJobs.Jobs
{
    public interface IJobDispatcher
    {
        Task Dispatch(Message message);
    }
}