using JobSystem.Queue;
using System.Threading.Tasks;

namespace JobSystem.Jobs
{
    public abstract class Job : IJob
    {
        public abstract Task<bool> ExecuteAsync(QueueMessage message);

        public string GetName()
        {
            return GetType().FullName;
        }
    }
}
