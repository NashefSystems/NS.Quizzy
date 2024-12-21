using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace NS.Quizzy.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GlobalErrorResponse))]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(GlobalErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest loginRequest)
        {
            var res = await _accountService.LoginAsync(loginRequest);
            if (res == null)
            {
                return Unauthorized(new GlobalErrorResponse()
                {
                    Message = "Invalid credentials"
                });
            }
            return Ok(res);
        }
    }
}