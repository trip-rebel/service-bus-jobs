using System;
using System.Threading.Tasks;

namespace ServiceBusJobs.Jobs
{
    public interface IJobScheduler
    {
        Task Schedule<T>(Type type, T body);
    }
}