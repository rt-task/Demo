using Microsoft.AspNetCore.Mvc;

namespace IdentityDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private readonly ILogger<HealthCheckController> _logger;

        public HealthCheckController(ILogger<HealthCheckController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Get()
        {
            _logger.LogInformation("Health check at {0}, host: {1}", DateTime.UtcNow, Request.Host);
            return Ok();
        }
    }
}