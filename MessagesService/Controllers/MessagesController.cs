using Microsoft.AspNetCore.Mvc;

namespace MessagesService.Controllers
{
    [ApiController]
    [Route("messages-service")]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(ILogger<MessagesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Not implemented yet");
        }
    }
}