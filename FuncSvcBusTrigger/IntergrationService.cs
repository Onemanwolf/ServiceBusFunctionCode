using Microsoft.Extensions.Options;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Files.DataLake;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using Azure;
using System.Linq;
using Microsoft.Identity.Web;

namespace FuncSvcBusTrigger
{
    public class IntergrationService
    {
        private string _dataLakeConnectionString;

        private ILogger<IntergrationService> _logger;
        private IOptions<ServiceConfiguration> _serviceConfiguration;

        private IHttpClientFactory _httpFactory;
        private HttpClient _httpClient;
        private string _container;    // _configuration["container"]; //"my-file-system";//await CreateFileSystemAsync(_dataLakeServiceClient);
        private string _directory;   //"my-directory"; //await CreateDirectoryAsync(_dataLakeServiceClient, container.Name);
        ServiceBusClient _client;
        private ITokenAcquisition _tokenAcquisition;

        private string _subdirectory;


        public IntergrationService(IOptions<ServiceConfiguration> serviceConfiguration, IHttpClientFactory httpFactory, ILogger<IntergrationService> logger, ITokenAcquisition tokenAcquisition)
        {
            _tokenAcquisition = tokenAcquisition;
            _serviceConfiguration = serviceConfiguration;
            _httpFactory = httpFactory;
            _httpClient = CreateHttpClient();
            _logger = logger;
            _dataLakeConnectionString = _serviceConfiguration.Value.DataLakeConnectionString;
            _container = _serviceConfiguration.Value.DataLakeContainerName;
            _directory = _serviceConfiguration.Value.DirectoryName;
            _subdirectory = _serviceConfiguration.Value.SubDirectoryName;
        }
        private HttpClient CreateHttpClient()
        {
            _httpClient = _httpFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_serviceConfiguration.Value.FhirEndpoint);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            // call to get bearer token

            // _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _serviceConfiguration.FhirToken);

            return _httpClient;



        }


        public async Task StartAsync(string message)
        {
            var encounter = message;
            //  get patient id from message
            var patientId = await GetPatientIdAysnc(_serviceConfiguration.Value.JsonPath, encounter);


            //Call Fhir Endpoint to get data
            var result = await GetEncounterAsync(message);
            //Upload to DataLake
            await UploadToDataLakeAsync(_container, result);

        }

        private async Task<JToken>? GetPatientIdAysnc(string jpath, string json)
        {
            JToken jToken = JToken.Parse(json);
            return await Task.Run(() => jToken.Any() ? jToken.SelectToken(jpath) : null); // true

        }

        private async Task<string> GetEncounterAsync(string messeageId)
        {

            var content = " ";
            try
            {
                // get token  from token cache
                var response = await _httpClient.GetAsync("Encounter");
                _logger.LogInformation($"Response: {response.StatusCode} time: {DateTimeOffset.Now}");
                content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Response: {content} time: {DateTimeOffset.Now}");

            }
            catch (Exception ex) { _logger.LogError(ex.Message); }
            return content;

        }

        private async Task UploadToDataLakeAsync(string fileSystemName, string encounter)
        {


            DataLakeServiceClient _dataLakeServiceClient = new DataLakeServiceClient(_dataLakeConnectionString);
            _dataLakeServiceClient.GetFileSystemClient(_container);
            DataLakeDirectoryClient directoryClient =
                    _dataLakeServiceClient.GetFileSystemClient(fileSystemName).GetDirectoryClient(_directory).GetSubDirectoryClient(_subdirectory);

            var filename = Guid.NewGuid().ToString() + ".json";
            DataLakeFileClient fileClient = directoryClient.GetFileClient(filename);



            await using var ms = new MemoryStream();
            var json = JsonConvert.SerializeObject(encounter);
            var writer = new StreamWriter(ms);
            await writer.WriteAsync(json);
            await writer.FlushAsync();
            ms.Position = 0;
            try
            {
                long fileSize = ms.Length;

                await fileClient.UploadAsync(ms, false);

                await fileClient.FlushAsync(position: fileSize);
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex.Message);
            }

        }



    }
}