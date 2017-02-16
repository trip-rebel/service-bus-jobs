using Amqp;
using JobSystem.Jobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Text;

namespace JobSystem.Queue
{
    public class QueueListener : QueueClient, IQueueListener
    {
        private ReceiverLink receiver;

        public Action<QueueMessage> OnMessage { get; set; }

        public QueueListener(IOptions<JobSystemOptions> optionsAccessor,
            IJobDispatcher dispatcher,
            ILogger<QueueListener> logger) : base(optionsAccessor, logger)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }
            
            var options = optionsAccessor.Value;

            if (options.EnableListener)
            {
                OnMessage = async (message) =>
                {
                    if (message.Body == null) return;
                    
                    await dispatcher.Dispatch(message);
                };
            }

            renewSession();
        }

        protected override void Renew(Session session, string serviceBusName)
        {
            // when no message callback is provided, simply do not construct SB connection
            if(OnMessage == null)
            {
                return;
            }

            receiver = new ReceiverLink(session, "receiver", serviceBusName);

            receiver.Start(20, (receiver, message) =>
            {
                try
                {
                    QueueMessage queueMessage = null;

                    if (message.Body.GetType() == typeof(byte[]))
                    {
                        var byteMessage = Encoding.UTF8.GetString(message.GetBody<byte[]>());
                        queueMessage = JsonConvert.DeserializeObject<QueueMessage>(byteMessage);
                    }
                    else
                    {
                        queueMessage = JsonConvert.DeserializeObject<QueueMessage>(message.GetBody<string>());
                    }

                    OnMessage(queueMessage);

                    receiver.Accept(message);
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);

                    receiver.Reject(message);
                }
            });
        }
    }
}
