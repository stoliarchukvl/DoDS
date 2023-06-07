using System.Text;
using System.Text.Json;
using Consul;
using Hazelcast;
using MessagesService.Models;
using Microsoft.AspNetCore.Mvc;

namespace MessagesService.Controllers
{
    [ApiController]
    [Route("messages-service")]
    public class MessagesController : ControllerBase
    {
        private static Dictionary<Guid, string> _table = new();
        private readonly ILogger<MessagesController> _logger;
        private readonly IConsulClient _consulClient;

        public MessagesController(ILogger<MessagesController> logger, IConsulClient consulClient)
        {
            _logger = logger;
            _consulClient = consulClient;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var response = string.Join(", ", _table.Values);
            return Ok(response);
        }

        [HttpPost]
        public IActionResult StartConsume()
        {
            var thread = new Thread(async () => await RunConsume());
            thread.Start();
            return Ok();
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok();
        }

        private async Task RunConsume()
        {
            var data = await _consulClient.KV.Get("hazelcast_queue");

            var hzQueue = Encoding.UTF8.GetString(data.Response.Value).Replace("\"", string.Empty);

            var options = new HazelcastOptionsBuilder().Build();
            options.ClusterName = "cluster";

            var client = await HazelcastClientFactory.StartNewClientAsync(options);

            var queue = await client.GetQueueAsync<string>(hzQueue);

            while (true)
            {
                var head = await queue.TakeAsync();
                if (string.IsNullOrEmpty(head))
                {
                    continue;
                }

                var message = JsonSerializer.Deserialize<LoggingMessage>(head);

                _table.Add(message.Id, message.Message);
                _logger.LogInformation("Message " + head);
            }
        }
    }
}