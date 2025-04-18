using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NS.Quizzy.Server.BL.Attributes;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.DTOs;
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
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectsService _service;
        public SubjectsController(ISubjectsService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<SubjectDto>))]
        public async Task<ActionResult<List<SubjectDto>>> GetAllAsync()
        {
            var res = await _service.GetAllAsync();
            return Ok(res);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SubjectDto))]
        public async Task<ActionResult<SubjectDto>> GetAsync(Guid id)
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
        [SwaggerResponse(StatusCodes.Status201Created, null, typeof(SubjectDto))]
        public async Task<ActionResult<SubjectDto>> InsertAsync([FromBody] SubjectPayloadDto payload)
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
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(SubjectDto))]
        public async Task<ActionResult<SubjectDto>> UpdateAsync(Guid id, [FromBody] SubjectPayloadDto payload)
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
        public async Task<ActionResult<SubjectDto>> DeleteAsync(Guid id)
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
