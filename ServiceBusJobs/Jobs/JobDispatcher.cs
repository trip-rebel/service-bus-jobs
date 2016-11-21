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

            try
            {
                _logger.LogInformation("Executing job {name}", message.Topic);

                var type = Type.GetType(message.Topic);
                var job = jobRegistry.Lookup(type);

                if (job == null)
                {
                    throw new ArgumentException($"Type '{message.Topic}' not found in service descriptor.");
                }

                await job.ExecuteAsync(message);
            } catch(Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
            finally {
                scope.Dispose();
            }
        }
    }
}
