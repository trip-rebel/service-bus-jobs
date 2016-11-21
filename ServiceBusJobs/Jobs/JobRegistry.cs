using System;

namespace JobSystem.Jobs
{
    public class JobRegistry : IJobRegistry
    {
        private IServiceProvider provider;

        public JobRegistry(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public IJob Lookup(Type type)
        {
            var job = (IJob)provider.GetService(type);

            return job;
        }
    }
}
