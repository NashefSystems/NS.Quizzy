using Microsoft.AspNetCore.Mvc;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace NS.Quizzy.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GlobalErrorResponse))]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;

        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        [HttpGet]
        public async Task<IActionResult> TestAsync()
        {
            var res = await _testService.TestAsync();
            return Ok(res);
        }
    }
}