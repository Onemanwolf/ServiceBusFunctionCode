namespace FuncSvcBusTrigger
{
    public class ServiceConfiguration
    {
        public string? ServiceBusConnectionString { get; set; }
        public string ServiceBusQueueName { get; set; }
        public string DataLakeConnectionString { get; set; }
        public string DataLakeContainerName { get; set; }
        public string DataLakeFileName { get; set; }
        public string DirectoryName { get; set; }
        public string SubDirectoryName { get; set; }
        public string FolderName { get; set; }
        public string FhirEndpoint { get; set; }
        public string FhirToken { get; set; }
        public string JsonPath { get; set; }

        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }
    }
}