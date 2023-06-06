using System.Text.Json;
using LoggingService.Models.Logging;
using Microsoft.AspNetCore.Mvc;

namespace LoggingService.Controllers
{
    [ApiController]
    [Route("logging-service")]
    public class LoggingController : ControllerBase
    {
        private static Dictionary<Guid, string> _table = new();
        private readonly ILogger<LoggingController> _logger;

        public LoggingController(ILogger<LoggingController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetLogs()
        {
            var response = string.Join(", ", _table.Values);
            return Ok(response);
        }

        [HttpPost]
        public IActionResult CreateLog([FromBody] LoggingRequest request)
        {
            _table.Add(request.Id, request.Message);
            _logger.LogInformation($"Message: {JsonSerializer.Serialize(request)}");
            return Ok();
        }
    }
}
