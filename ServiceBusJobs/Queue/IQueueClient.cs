using System.Threading.Tasks;

namespace JobSystem.Queue
{
    public interface IQueueSender
    {
        Task SendAsync(QueueMessage message);
    }
}
