using System.Threading.Tasks;

namespace ServiceBusJobs.Jobs
{
    public interface IJobListener
    {
        Task WaitAndListen();
    }
}
