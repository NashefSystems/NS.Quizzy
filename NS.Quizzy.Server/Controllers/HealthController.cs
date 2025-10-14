using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Shared.Logging.Attributes;
namespace NS.Quizzy.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [IgnoreLoggingAPICallInfo]
    public class HealthController : ControllerBase
    {
        private readonly IHealthService _service;

        public HealthController(IHealthService service)
        {
            _service = service;
        }

        /// <summary>
        /// Liveness probe - checks if the application is running
        /// Returns 200 if alive, 503 if dead
        /// </summary>
        [HttpGet("Liveness")]
        public IActionResult Liveness()
        {
            return Ok(new { status = "Healthy" });
        }

        /// <summary>
        /// Readiness probe - checks if the application is ready to accept traffic
        /// Returns 200 if Healthy or Degraded, 503 if Unhealthy
        /// </summary>
        [HttpGet("Readiness")]
        public async Task<IActionResult> ReadinessAsync()
        {
            var result = await _service.ReadinessAsync();

            return result.Status switch
            {
                HealthStatus.Healthy => Ok(result),
                HealthStatus.Degraded => Ok(result), // Still accepts traffic but with warning
                HealthStatus.Unhealthy => StatusCode(503, result),
                _ => StatusCode(503, result)
            };
        }
    }
}
