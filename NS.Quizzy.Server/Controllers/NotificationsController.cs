using Microsoft.AspNetCore.Mvc;
using NS.Quizzy.Server.BL.Attributes;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.Models.Models;
using NS.Shared.Logging.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace NS.Quizzy.Server.Controllers
{
    [ApiController]
    //[Authorize]
    //[RoleRequirement(DAL.DALEnums.Roles.Student)]
    [Route("api/[controller]")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GlobalErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GlobalErrorResponse))]
    [LoggingAPICallInfo]
    public class NotificationsController : ControllerBase
    {
        private readonly IFcmService _service;
        public NotificationsController(IFcmService service)
        {
            _service = service;
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<IActionResult> SendPushNotificationAsync(PushNotificationRequest request)
        {
            await _service.SendPushNotificationAsync(request);
            return Ok();
        }
    }
}
