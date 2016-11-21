using System;

namespace JobSystem.Jobs
{
    public interface IJobRegistry
    {
        IJob Lookup(Type type);
    }
}