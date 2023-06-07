using System.Text;
using System.Text.Json;
using Consul;
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
        private readonly IConsulClient _consulClient;

        public LoggingController(ILogger<LoggingController> logger, IConsulClient consulClient)
        {
            _logger = logger;
            _consulClient = consulClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            var data = await _consulClient.KV.Get("hazelcast_map");

            var hzMap = Encoding.UTF8.GetString(data.Response.Value).Replace("\"", string.Empty);
            var options = new HazelcastOptionsBuilder().Build();
            options.ClusterName = "cluster";

            var client = await HazelcastClientFactory.StartNewClientAsync(options);
            var map = await client.GetMapAsync<string, string>(hzMap);

            var response = string.Join(", ", await map.GetValuesAsync());
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLog([FromBody] LoggingRequest request)
        {
            var data = await _consulClient.KV.Get("hazelcast_map");

            var hzMap = Encoding.UTF8.GetString(data.Response.Value).Replace("\"", string.Empty);
            var options = new HazelcastOptionsBuilder().Build();
            options.ClusterName = "cluster";

            var client = await HazelcastClientFactory.StartNewClientAsync(options);
            var map = await client.GetMapAsync<string, string>(hzMap);

            await map.PutAsync(request.Id.ToString(), request.Message);
            
            _logger.LogInformation($"Message: {JsonSerializer.Serialize(request)}");
            return Ok();
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok();
        }
    }
}
