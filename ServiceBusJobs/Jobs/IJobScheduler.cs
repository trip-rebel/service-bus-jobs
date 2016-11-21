using System;
using System.Threading.Tasks;

namespace JobSystem.Jobs
{
    public interface IJobScheduler
    {
        Task Schedule<T>(Type type, T body);
    }
}