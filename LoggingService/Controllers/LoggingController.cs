using System.Text.Json;
using Hazelcast;
using LoggingService.Models.Logging;
using Microsoft.AspNetCore.Mvc;

namespace LoggingService.Controllers
{
    [ApiController]
    [Route("logging-service")]
    public class LoggingController : ControllerBase
    {
        private readonly ILogger<LoggingController> _logger;

        public LoggingController(ILogger<LoggingController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            var options = new HazelcastOptionsBuilder().Build();
            options.ClusterName = "cluster";

            var client = await HazelcastClientFactory.StartNewClientAsync(options);
            var map = await client.GetMapAsync<string, string>("my-distributed-map");

            var response = string.Join(", ", await map.GetValuesAsync());
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLog([FromBody] LoggingRequest request)
        {
            var options = new HazelcastOptionsBuilder().Build();
            options.ClusterName = "cluster";

            var client = await HazelcastClientFactory.StartNewClientAsync(options);
            var map = await client.GetMapAsync<string, string>("my-distributed-map");
            
            await map.PutAsync(request.Id.ToString(), request.Message);
            
            _logger.LogInformation($"Message: {JsonSerializer.Serialize(request)}");
            return Ok();
        }
    }
}
