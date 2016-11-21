using Amqp;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using JobSystem.Jobs;

namespace JobSystem.Queue
{
    public abstract class QueueClient : IDisposable
    {
        private Session session;
        private Connection connection;

        private CancellationTokenSource cancellationTokenSource;

        private readonly Address address;
        private readonly string serviceBusName;

        protected abstract void Renew(Session session, string serviceBusName);

        protected QueueClient(IOptions<JobSystemOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            var options = optionsAccessor.Value;

            address = new Address(options.ServiceBusUrl,
                5671,
                options.ServiceBusSAKPolicyName,
                options.ServiceBusSAKSharedSecret);

            serviceBusName = options.ServiceBusName;

            cancellationTokenSource = new CancellationTokenSource();

            renewSession();
        }
        private void renewSession()
        {
            connection = new Connection(address);
            session = new Session(connection);

            Renew(session, serviceBusName);

            startRenewTimer();
        }

        private void startRenewTimer()
        {
            delayedRenewSession(cancellationTokenSource.Token);
        }

        private async Task delayedRenewSession(CancellationToken token)
        {
            await Task.Delay(5 * 60 * 1000, token);

            if (!token.IsCancellationRequested)
                renewSession();
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            
            cancellationTokenSource.Dispose();
        }
    }
}
