namespace JobSystem.Jobs
{
    public class JobSystemOptions
    {
        public string ServiceBusName { get; set; }
        public string ServiceBusUrl { get; set; }
        public string ServiceBusSAKPolicyName { get; set; }
        public string ServiceBusSAKSharedSecret { get; set; }
        public bool EnableListener { get; set; }
    }
}
