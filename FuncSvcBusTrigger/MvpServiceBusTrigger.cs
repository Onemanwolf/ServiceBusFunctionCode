using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FuncSvcBusTrigger
{

    public class MvpServiceBusTrigger
    {
        private IntergrationService _intergrationService;
        private readonly IOptions<ServiceConfiguration> _serviceConfiguration;
        private readonly ILogger<MvpServiceBusTrigger> _logger;
        private  readonly string _subscriptionName;

        public MvpServiceBusTrigger(ILogger<MvpServiceBusTrigger> log, IntergrationService intergrationService, IOptions<ServiceConfiguration> serviceConfiguration)
        {
            _logger = log;
            _intergrationService = intergrationService;
            _serviceConfiguration = serviceConfiguration;

        }

        [FunctionName("MvpServiceBusTrigger")]
        public async Task Run([ServiceBusTrigger( "mysync", "encountersync", Connection = "ServiceBusConnectionString")]string mySbMsg)
        {
          await _intergrationService.StartAsync(mySbMsg);
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }
    }
}
