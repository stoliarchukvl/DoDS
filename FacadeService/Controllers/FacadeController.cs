using System.Text;
using System.Text.Json;
using Consul;
using FacadeService.Abstractions.Providers;
using FacadeService.Contracts.Logging;
using Hazelcast;
using Microsoft.AspNetCore.Mvc;

namespace FacadeService.Controllers
{
    [ApiController]
    [Route("facade-service")]
    public class FacadeController : ControllerBase
    {
        private readonly ILoggingServiceProvider _loggingServiceProvider;
        private readonly IMessagesServiceProvider _messagesServiceProvider;
        private readonly IConsulClient _consulClient;
        private readonly ILogger<FacadeController> _logger;

        public FacadeController(
            ILoggingServiceProvider loggingServiceProvider,
            IMessagesServiceProvider messagesServiceProvider,
            IConsulClient consulClient,
            ILogger<FacadeController> logger)
        {
            _loggingServiceProvider = loggingServiceProvider;
            _messagesServiceProvider = messagesServiceProvider;
            _consulClient = consulClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            var loggingServiceResponse = await _loggingServiceProvider.GetLogs();
            var messagesServiceResponse = await _messagesServiceProvider.Get();
            var response = new GetLogsResponse(loggingServiceResponse, messagesServiceResponse);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLog([FromBody] string message)
        {
            var loggingRequest = new LoggingRequest(Guid.NewGuid(), message);
            await _loggingServiceProvider.CreateLog(loggingRequest);

            var data = await _consulClient.KV.Get("hazelcast_queue");

            var hzQueue = Encoding.UTF8.GetString(data.Response.Value).Replace("\"", string.Empty);

            var options = new HazelcastOptionsBuilder().Build();
            options.ClusterName = "cluster";

            var client = await HazelcastClientFactory.StartNewClientAsync(options);

            var queue = await client.GetQueueAsync<string>(hzQueue);

            await queue.PutAsync(JsonSerializer.Serialize(loggingRequest));

            return Ok();
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok("health");
        }
    }
}