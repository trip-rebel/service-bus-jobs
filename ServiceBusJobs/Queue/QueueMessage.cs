using Newtonsoft.Json;

namespace JobSystem.Queue
{
    public class QueueMessage
    {
        public QueueMessage() { }

        public QueueMessage(string topic) {
            Topic = topic;
        }

        public string Topic { get; set; }
        public string Body { get; set; }

        public T GetBody<T>()
        {
            return JsonConvert.DeserializeObject<T>(Body);
        }

        public void SetBody<T>(T body)
        {
            Body = JsonConvert.SerializeObject(body);
        }
    }
}
