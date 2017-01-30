using Amqp;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using JobSystem.Jobs;
using Microsoft.Extensions.Logging;

namespace JobSystem.Queue
{
    public abstract class QueueClient : IDisposable
    {
        private Session session;
        private Connection connection;

        private CancellationTokenSource cancellationTokenSource;

        private readonly Address address;
        private readonly string serviceBusName;
        private readonly ILogger<QueueClient> logger;

        protected abstract void Renew(Session session, string serviceBusName);

        protected QueueClient(IOptions<JobSystemOptions> optionsAccessor,
            ILogger<QueueClient> logger)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            this.logger = logger;

            var options = optionsAccessor.Value;

            address = new Address(options.ServiceBusUrl,
                5671,
                options.ServiceBusSAKPolicyName,
                options.ServiceBusSAKSharedSecret);

            serviceBusName = options.ServiceBusName;

            cancellationTokenSource = new CancellationTokenSource();
        }

        protected void renewSession()
        {
            logger.LogInformation("Renewing session");

            connection = new Connection(address);
            session = new Session(connection);

            session.Closed += Session_Closed;

            Renew(session, serviceBusName);
        }

        private void Session_Closed(AmqpObject client, Amqp.Framing.Error error)
        {
            if (client.Error != null)
            {
                logger.LogError($"Closed connection with error code {client.Error.Condition}: {client.Error.Description}");
            }

            if (!cancellationTokenSource.IsCancellationRequested)
            {
                renewSession();
            }
        }
        
        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            
            cancellationTokenSource.Dispose();
        }
    }
}
