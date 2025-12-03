using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NS.Quizzy.Server.BL.Attributes;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.Models;
using Swashbuckle.AspNetCore.Annotations;
namespace NS.Quizzy.Server.Controllers
{
    [Authorize]
    [ApiController]
    [RoleRequirement(DAL.DALEnums.Roles.Admin)]
    [Route("api/[controller]")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GlobalErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GlobalErrorResponse))]
    public class SystemController : ControllerBase
    {
        private readonly ISystemService _service;

        public SystemController(ISystemService service)
        {
            _service = service;
        }

        [HttpPatch("ReQueueDlqMessages")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> ReQueueDlqMessagesAsync([FromQuery] string queueName)
        {
            var res = await _service.ReQueueDlqMessagesAsync(queueName);
            return Ok(res);
        }
    }
}
