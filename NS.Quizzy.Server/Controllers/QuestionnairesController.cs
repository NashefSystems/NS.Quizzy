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
    [Route("api/[controller]")]
    [Authorize]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GlobalErrorResponse))]
    [LoggingAPICallInfo]
    public class QuestionnairesController : ControllerBase
    {
        private readonly IQuestionnairesService _service;
        public QuestionnairesController(IQuestionnairesService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<QuestionnaireDto>))]
        public async Task<ActionResult<List<QuestionnaireDto>>> GetAllAsync()
        {
            var res = await _service.GetAllAsync();
            return Ok(res);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(QuestionnaireDto))]
        public async Task<ActionResult<QuestionnaireDto>> GetAsync(Guid id)
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
        [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GlobalErrorResponse))]
        [SwaggerResponse(StatusCodes.Status201Created, null, typeof(QuestionnaireDto))]
        public async Task<ActionResult<QuestionnaireDto>> InsertAsync([FromBody] QuestionnairePayloadDto payload)
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
        [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GlobalErrorResponse))]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(QuestionnaireDto))]
        public async Task<ActionResult<QuestionnaireDto>> UpdateAsync(Guid id, [FromBody] QuestionnairePayloadDto payload)
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
        [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GlobalErrorResponse))]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<QuestionnaireDto>> DeleteAsync(Guid id)
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