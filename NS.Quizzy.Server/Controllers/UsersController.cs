using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NS.Quizzy.Server.BL.Attributes;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.DTOs;
using NS.Quizzy.Server.Models.Models;
using NS.Shared.Logging.Attributes;
using NS.Shared.Logging.Extensions;
using Swashbuckle.AspNetCore.Annotations;

namespace NS.Quizzy.Server.Controllers
{
    [ApiController]
    [Authorize]
    [RoleRequirement(DAL.DALEnums.Roles.Admin)]
    [Route("api/[controller]")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GlobalErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GlobalErrorResponse))]
    [LoggingAPICallInfo]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _service;
        public UsersController(IUsersService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<UserDto>))]
        public async Task<ActionResult<List<UserDto>>> GetAllAsync()
        {
            var res = await _service.GetAllAsync();
            return Ok(res);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(UserDto))]
        public async Task<ActionResult<UserDto>> GetAsync(Guid id)
        {
            var res = await _service.GetAsync(id);
            if (res == null)
            {
                return NotFound("Item not found");
            }
            return Ok(res);
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, null, typeof(UserDto))]
        public async Task<ActionResult<UserDto>> InsertAsync([FromBody] UserPayloadDto payload)
        {
            var res = await _service.InsertAsync(payload);
            if (res == null)
            {
                return BadRequest("Failed to insert item.");
            }

            var getUri = $"{Request.GetFullURL()}/{res.Id}";
            return Created(getUri, res);
        }

        [HttpPut("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(UserDto))]
        public async Task<ActionResult<UserDto>> UpdateAsync(Guid id, [FromBody] UserPayloadDto payload)
        {
            var res = await _service.UpdateAsync(id, payload);
            if (res == null)
            {
                return NotFound("Item not found");
            }
            return Ok(res);
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<UserDto>> DeleteAsync(Guid id)
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
