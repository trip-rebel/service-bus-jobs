using System;
using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ServiceBusJobs.Jobs
{
    public class JobListener : IJobListener
    {
        private QueueClient queueClient;
        private IJobDispatcher dispatcher;
        private ILogger<JobListener> logger;
        private TaskCompletionSource<bool> quitEvent;

        public JobListener(IJobDispatcher dispatcher,
            ILogger<JobListener> logger,
            IOptions<JobSystemOptions> optionsAccessor)
        {
            queueClient = new QueueClient(optionsAccessor.Value.ServiceBusConnectionString,
                optionsAccessor.Value.ServiceBusName,
                ReceiveMode.PeekLock);

            this.dispatcher = dispatcher;
            this.logger = logger;

            quitEvent = new TaskCompletionSource<bool>();
        }

        public async Task WaitAndListen()
        {
            Console.CancelKeyPress += (sender, eArgs) => {
                quitEvent.TrySetCanceled();
                eArgs.Cancel = true;
            };

            queueClient.RegisterMessageHandler(async (message, token) =>
            {
                try
                {
                    await dispatcher.Dispatch(message);

                    await queueClient.CompleteAsync(message.SystemProperties.LockToken);
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);

                    await queueClient.AbandonAsync(message.SystemProperties.LockToken);
                }
            },
            new RegisterHandlerOptions() { MaxConcurrentCalls = 20, AutoComplete = false });
            
            await quitEvent.Task;
            
            await queueClient.CloseAsync();
        }
    }
}
