using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NS.Quizzy.Server.BL.Attributes;
using NS.Quizzy.Server.BL.DTOs;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.Models;
using NS.Shared.Logging.Extensions;
using Swashbuckle.AspNetCore.Annotations;

namespace NS.Quizzy.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GlobalErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GlobalErrorResponse))]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationsService _service;
        public NotificationsController(INotificationsService service)
        {
            _service = service;
        }

        [HttpGet("NumberOfMyNewNotifications")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(int))]
        public async Task<ActionResult<int>> GetNumberOfMyNewNotificationsAsync()
        {
            var res = await _service.GetNumberOfMyNewNotificationsAsync();
            return Ok(res);
        }

        [HttpGet("MyNotifications")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<MyNotificationItem>))]
        public async Task<ActionResult<List<MyNotificationItem>>> GetMyNotificationsAsync([FromQuery] int? limit = null)
        {
            var res = await _service.GetMyNotificationsAsync(limit);
            return Ok(res);
        }

        [HttpPut("{notificationId}/read")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<MyNotificationItem>))]
        public async Task<ActionResult<MyNotificationItem>> MarkAsReadAsync(Guid notificationId)
        {
            var res = await _service.MarkAsReadAsync(notificationId);
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


        [HttpDelete("{id}")]
        [RoleRequirement(DAL.DALEnums.Roles.Admin)]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<NotificationDto>> DeleteAsync(Guid id)
        {
            var res = await _service.DeleteAsync(id);
            if (!res)
            {
                return NotFound("Item not found");
            }
            return NoContent();
        }
    }
}
