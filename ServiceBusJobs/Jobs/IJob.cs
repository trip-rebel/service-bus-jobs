using JobSystem.Queue;
using System.Threading.Tasks;

namespace JobSystem.Jobs
{
    public interface IJob
    {
        Task<bool> ExecuteAsync(QueueMessage message);
        string GetName();
    }
}