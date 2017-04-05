using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using ServiceBusJobs.Jobs;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JobSystemServiceCollectionExtensions
    {
        public static IServiceCollection AddJobSystem(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            
            services.TryAdd(ServiceDescriptor.Singleton<IJobScheduler, JobScheduler>());
            services.TryAdd(ServiceDescriptor.Singleton<IJobDispatcher, JobDispatcher>());
            services.TryAdd(ServiceDescriptor.Singleton<IJobListener, JobListener>());

            return services;
        }

        public static IServiceCollection AddJobSystem(this IServiceCollection services,
            Action<JobSystemOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddJobSystem();
            services.Configure(setupAction);

            return services;
        }
    }
}
