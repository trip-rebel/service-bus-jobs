using Amqp;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using JobSystem.Jobs;
using Microsoft.Extensions.Logging;

namespace JobSystem.Queue
{
    public class QueueSender : QueueClient, IQueueSender
    {
        private readonly ILogger<QueueSender> logger;
        private SenderLink sender;

        public QueueSender(IOptions<JobSystemOptions> optionsAccessor,
            ILogger<QueueSender> logger) : base(optionsAccessor, logger)
        {
            this.logger = logger;
        }

        public async Task SendAsync(QueueMessage queueMessage)
        {
            if(sender.IsClosed)
            {
                renewSession();
            }

            await sender.SendAsync(new Message(JsonConvert.SerializeObject(queueMessage)));

            if (sender.Error != null) {
                logger.LogError($"Failed sending message with error code {sender.Error.Condition}: {sender.Error.Description}");
            }
        }

        protected override void Renew(Session session, string serviceBusName)
        {
            sender = new SenderLink(session, "sender", serviceBusName);
        }
    }
}
