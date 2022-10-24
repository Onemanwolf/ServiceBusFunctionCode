## Local Host Settings example for Azure Functions

- local.settings.json file is used to store the connection string for the Azure Service Bus example provided below.

```json
"IsEncrypted": false,
   "Values": {
       "ServiceConfiguration:AzureWebJobsStorage": "UseDevelopmentStorage=true",
       "FUNCTIONS_WORKER_RUNTIME": "dotnet",
       "ServiceConfiguration:ServiceBusConnectionString": "Your connection string goes here",
       "ServiceConfiguration:DataLakeConnectionString": "Your connection string goes here",
       "ServiceConfiguration:TopicName": "mysync",
       "ServiceConfiguration:SubscriptionName": "encountersync",
       "ServiceConfiguration:DataLakeContainerName": "mvpfilesystem",
       "ServiceConfiguration:DataLakeFileName": "encounter.json",
       "ServiceConfiguration:DirectoryName": "mvpdirectory",
       "ServiceConfiguration:SubDirectoryName": "encounter",
       "ServiceConfiguration:FhirEndpoint": "http://vonk.fire.ly/",
       "ServiceConfiguration:JsonPath": "$.MemeberId"
   }
```
