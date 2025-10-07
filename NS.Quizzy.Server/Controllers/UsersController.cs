using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NS.Quizzy.Server.BL.Attributes;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.BL.DTOs;
using NS.Quizzy.Server.Models.Models;
using NS.Shared.Logging.Attributes;
using NS.Shared.Logging.Extensions;
using NS.Shared.QueueManager.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Text;

namespace NS.Quizzy.Server.Controllers
{
    [Authorize]
    [ApiController]
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

        [HttpGet("Download")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(File))]
        public async Task<ActionResult> DownloadAsync()
        {
            var bytes = await _service.DownloadAsync();
            return File(bytes, "text/csv", $"{DateTime.Now:yyyy-MM-dd HH-mm-ss} Quizzy users.csv");
        }

        [HttpPost("Upload")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<UploadFileResponse>> UploadFileAsync([FromForm] IFormFile file)
        {
            var res = await _service.UploadAsync(file);
            return Ok(res);
        }

        [HttpGet("UploadFileStatus/{uploadMessageId}")]
        public async Task<ActionResult<UploadFileStatusResponse>> UploadFileStatusAsync(Guid uploadMessageId)
        {
            var res = await _service.UploadFileStatusAsync(uploadMessageId);
            return Ok(res);
        }

        [HttpGet("UsersLoginStatus")]
        public async Task<ActionResult<List<UserLoginStatusDto>>> GetUsersLoginStatusAsync()
        {
            var res = await _service.GetUsersLoginStatusAsync();
            return Ok(res);
        }
    }
}
