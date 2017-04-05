using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceBusJobs.Jobs
{
    public class MessageContainer : IMessageContainer
    {
        private byte[] Body;

        public MessageContainer(Message message)
        {
            Body = message.Body;
        }

        public T GetBody<T>()
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(Body));
        }
    }
}
