using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.DTOs;
using NS.Quizzy.Server.Models.Models;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NS.Quizzy.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GlobalErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GlobalErrorResponse))]
    public class ExamsController : ControllerBase
    {
        private readonly IExamsService _examService;
        public ExamsController(IExamsService examService)
        {
            _examService = examService;
        }

        [HttpPost("Filter")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ExamDto>>> FilterAsync([FromBody] FilterRequest filter)
        {
            var res = await _examService.FilterAsync(filter);
            return Ok(res);
        }

        [HttpGet]
        public async Task<ActionResult<List<ExamDto>>> GetAllAsync()
        {
            var res = await _examService.GetAllAsync();
            return Ok(res);
        }
    }
}
