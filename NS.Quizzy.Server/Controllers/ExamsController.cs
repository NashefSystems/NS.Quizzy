using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.DTOs;
using NS.Quizzy.Server.Models.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace NS.Quizzy.Server.Controllers
{
    [ApiController]
    [Authorize]
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
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<ExamDto>))]
        public async Task<ActionResult<List<ExamDto>>> GetAllAsync([FromQuery(Name = "from")] DateTimeOffset? dtFrom, [FromQuery(Name = "to")] DateTimeOffset? dtTo, [FromQuery(Name = "classes")] List<Guid>? classIds, [FromQuery(Name = "subjects")] List<Guid>? subjectIds)
        {
            var res = await _service.FilterAsync(dtFrom, dtTo, classIds, subjectIds);
            return Ok(res);
        }
    }
}
