using Microsoft.AspNetCore.Mvc;
using NS.Quizzy.Server.BL.Attributes;
using NS.Quizzy.Server.BL.DTOs;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.Models;
using NS.Shared.Logging.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace NS.Quizzy.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GlobalErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GlobalErrorResponse))]
    [LoggingAPICallInfo]
    public class DevicesController : ControllerBase
    {
        private readonly IDevicesService _service;
        public DevicesController(IDevicesService service)
        {
            _service = service;
        }

        [HttpPatch]
        [RoleRequirement(DAL.DALEnums.Roles.Admin)]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(DeviceDto))]
        public async Task<ActionResult<DeviceDto>> UpdateInfoAsync([FromBody] DevicePayloadDto payload)
        {
            var res = await _service.UpdateInfoAsync(payload);
            return Ok(res);
        }
    }
}
