using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NS.Quizzy.Server.BL.Attributes;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.BL.DTOs;
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
    public class ExamsController : ControllerBase
    {
        private readonly IExamsService _service;
        public ExamsController(IExamsService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<ExamDto>))]
        public async Task<ActionResult<List<ExamDto>>> GetAllAsync([FromQuery] bool filterCompletedExams)
        {
            var res = await _service.GetAllAsync(filterCompletedExams);
            return Ok(res);
        }

        [HttpPost("Filter")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<ExamDto>))]
        public async Task<ActionResult<List<ExamDto>>> FilterAsync([FromBody] ExamFilterRequest request)
        {
            var res = await _service.FilterAsync(request);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ExamDto))]
        public async Task<ActionResult<ExamDto>> GetAsync(Guid id)
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
        [SwaggerResponse(StatusCodes.Status201Created, null, typeof(ExamDto))]
        public async Task<ActionResult<ExamDto>> InsertAsync([FromBody] ExamPayloadDto payload)
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
        [RoleRequirement(DAL.DALEnums.Roles.Admin)]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ExamDto))]
        public async Task<ActionResult<ExamDto>> UpdateAsync(Guid id, [FromBody] ExamPayloadDto payload)
        {
            var res = await _service.UpdateAsync(id, payload);
            if (res == null)
            {
                return NotFound("Item not found");
            }
            return Ok(res);
        }

        [HttpDelete("{id}")]
        [RoleRequirement(DAL.DALEnums.Roles.Admin)]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<ExamDto>> DeleteAsync(Guid id)
        {
            var res = await _service.DeleteAsync(id);
            if (!res)
            {
                return NotFound("Item not found");
            }
            return NoContent();
        }

        [HttpPatch("SetAsVisible/{id}")]
        [RoleRequirement(DAL.DALEnums.Roles.Admin)]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ExamDto))]
        public async Task<ActionResult<ExamDto>> SetAsVisibleAsync(Guid id)
        {
            var res = await _service.SetAsVisibleAsync(id);
            if (res == null)
            {
                return NotFound("Item not found");
            }

            return Ok(res);
        }

        [HttpPost("ReSyncEvents")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [RoleRequirement(DAL.DALEnums.Roles.Admin)]
        public async Task<ActionResult<ReSyncEventsResponse>> ReSyncEventsAsync([FromBody] ReSyncEventsRequest request)
        {
            var res = await _service.ReSyncEventsAsync(request);
            return Ok(res);
        }
    }
}
