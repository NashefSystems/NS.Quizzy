using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.DTOs;
using NS.Quizzy.Server.Models.Models;
using NS.Shared.Logging.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace NS.Quizzy.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GlobalErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GlobalErrorResponse))]
    [LoggingAPICallInfo]
    public class ClassesController : ControllerBase
    {
        private readonly IClassesService _service;
        public ClassesController(IClassesService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(List<SubjectDto>))]
        public async Task<ActionResult<List<SubjectDto>>> GetAllAsync()
        {
            var res = await _service.GetAllAsync();
            return Ok(res);
        }
    }
}