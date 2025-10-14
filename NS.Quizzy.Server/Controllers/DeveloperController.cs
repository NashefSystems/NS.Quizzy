#if DEBUG
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.Models;
using NS.Shared.Logging.Attributes;
using Swashbuckle.AspNetCore.Annotations;
namespace NS.Quizzy.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GlobalErrorResponse))]
    [IgnoreLoggingAPICallInfo]
    public class DeveloperController : ControllerBase
    {
        private readonly IDeveloperService _service;

        public DeveloperController(IDeveloperService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet("Test")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> TestAsync()
        {
            var res = await _service.TestAsync();
            return Ok(res);
        }
    }
}
#endif