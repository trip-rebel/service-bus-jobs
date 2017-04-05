using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceBusJobs.Jobs
{
    public interface IMessageContainer
    {
        T GetBody<T>();
    }
}
