using JobSystem.Queue;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace JobSystem.Jobs
{
    public class JobDispatcher : IJobDispatcher
    {
        private readonly IJobRegistry jobRegistry;
        private readonly ILogger _logger;

        public JobDispatcher(IJobRegistry jobRegistry, ILogger<JobDispatcher> logger)
        {
            this.jobRegistry = jobRegistry;
            _logger = logger;
        }

        public async Task Dispatch(QueueMessage message)
        {
            var scope = _logger.BeginScope(message);
            
            _logger.LogInformation($"Executing job {message.Topic}");

            var type = Type.GetType(message.Topic);
            var job = jobRegistry.Lookup(type);

            if (job == null)
            {
                throw new ArgumentException($"Type '{message.Topic}' not found in service descriptor.");
            }

            if(!await job.ExecuteAsync(message))
            {
                throw new Exception($"Job {message.Topic} was unsuccesful.");
            }

            scope.Dispose();
        }
    }
}
