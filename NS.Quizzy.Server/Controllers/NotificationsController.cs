using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NS.Quizzy.Server.BL.Attributes;
using NS.Quizzy.Server.BL.DTOs;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.Models;
using NS.Shared.Logging.Attributes;
using NS.Shared.Logging.Extensions;
using Swashbuckle.AspNetCore.Annotations;

namespace NS.Quizzy.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GlobalErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GlobalErrorResponse))]
    [LoggingAPICallInfo]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationsService _service;
        public NotificationsController(INotificationsService service)
        {
            _service = service;
        }

        [HttpGet("MyNotifications")]        
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<NotificationDto>))]
        public async Task<ActionResult<List<NotificationDto>>> GetMyNotificationsAsync([FromQuery] bool isArchive = false)
        {
            var res = await _service.GetMyNotificationsAsync(isArchive);
            return Ok(res);
        }

        [HttpGet]
        [RoleRequirement(DAL.DALEnums.Roles.Admin)]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<NotificationDto>))]
        public async Task<ActionResult<List<NotificationDto>>> GetAllAsync()
        {
            var res = await _service.GetAllAsync();
            return Ok(res);
        }

        [HttpGet("{id}")]
        [RoleRequirement(DAL.DALEnums.Roles.Admin)]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(NotificationDto))]
        public async Task<ActionResult<NotificationDto>> GetAsync(Guid id)
        {
            var res = await _service.GetAsync(id);
            if (res == null)
            {
                return NotFound("Item not found");
            }
            return Ok(res);
        }

        [HttpPost]
        [RoleRequirement(DAL.DALEnums.Roles.Admin)]
        [SwaggerResponse(StatusCodes.Status201Created, null, typeof(NotificationDto))]
        public async Task<ActionResult<NotificationDto>> InsertAsync([FromBody] NotificationPayloadDto payload)
        {
            var res = await _service.InsertAsync(payload);
            if (res == null)
            {
                return BadRequest("Failed to insert item.");
            }

            var getUri = $"{Request.GetFullURL()}/{res.Id}";
            return Created(getUri, res);
        }
    }
}
