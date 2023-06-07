using System.Text.Json;
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
        private readonly ILogger<FacadeController> _logger;

        public FacadeController(
            ILoggingServiceProvider loggingServiceProvider,
            IMessagesServiceProvider messagesServiceProvider,
            ILogger<FacadeController> logger)
        {
            _loggingServiceProvider = loggingServiceProvider;
            _messagesServiceProvider = messagesServiceProvider;
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

            var options = new HazelcastOptionsBuilder().Build();
            options.ClusterName = "cluster";

            var client = await HazelcastClientFactory.StartNewClientAsync(options);

            var queue = await client.GetQueueAsync<string>("queue");

            await queue.PutAsync(JsonSerializer.Serialize(loggingRequest));

            return Ok();
        }
    }
}