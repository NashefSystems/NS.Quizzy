using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Route("api/[controller]")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GlobalErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GlobalErrorResponse))]
    [LoggingAPICallInfo]
    public class MoedsController : ControllerBase
    {
        private readonly IMoedsService _service;
        public MoedsController(IMoedsService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<MoedDto>))]
        public async Task<ActionResult<List<MoedDto>>> GetAllAsync()
        {
            var res = await _service.GetAllAsync();
            return Ok(res);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(MoedDto))]
        public async Task<ActionResult<MoedDto>> GetAsync(Guid id)
        {
            var res = await _service.GetAsync(id);
            if (res == null)
            {
                return NotFound("Item not found");
            }
            return Ok(res);
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, null, typeof(MoedDto))]
        public async Task<ActionResult<MoedDto>> InsertAsync([FromBody] MoedPayloadDto payload)
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
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(MoedDto))]
        public async Task<ActionResult<MoedDto>> UpdateAsync(Guid id, [FromBody] MoedPayloadDto payload)
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
        public async Task<ActionResult<MoedDto>> DeleteAsync(Guid id)
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
