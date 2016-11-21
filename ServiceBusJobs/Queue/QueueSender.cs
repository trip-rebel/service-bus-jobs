using Amqp;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using JobSystem.Jobs;

namespace JobSystem.Queue
{
    public class QueueSender : QueueClient, IQueueSender
    {
        private SenderLink sender;

        public QueueSender(IOptions<JobSystemOptions> optionsAccessor) : base(optionsAccessor) { }

        public async Task SendAsync(QueueMessage queueMessage)
        {
            await sender.SendAsync(new Message(JsonConvert.SerializeObject(queueMessage)));
        }

        protected override void Renew(Session session, string serviceBusName)
        {
            sender = new SenderLink(session, "sender", serviceBusName);
        }
    }
}
