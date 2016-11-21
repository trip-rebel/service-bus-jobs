using JobSystem.Queue;
using System.Threading.Tasks;

namespace JobSystem.Jobs
{
    public interface IJobDispatcher
    {
        Task Dispatch(QueueMessage message);
    }
}