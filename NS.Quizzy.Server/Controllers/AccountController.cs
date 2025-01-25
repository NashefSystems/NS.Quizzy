using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.Models.Models;
using NS.Shared.Logging.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace NS.Quizzy.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, null, typeof(GlobalErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GlobalErrorResponse))]
    [LoggingAPICallInfo]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("Login")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(LoginResponse))]
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

        [HttpPost("VerifyOTP")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(UserDetailsDto))]
        public async Task<ActionResult<UserDetailsDto>> LoginAsync([FromBody] VerifyOTPRequest request)
        {
            var res = await _accountService.VerifyOTP(request);
            if (res == null)
            {
                return BadRequest(new GlobalErrorResponse()
                {
                    Message = "Invalid OTP"
                });
            }
            return Ok(res);
        }

        [HttpGet("Details")]
        [Authorize]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(UserDetailsDto))]
        public async Task<ActionResult<UserDetailsDto>> GetDetailsAsync()
        {
            var res = await _accountService.GetDetailsAsync();
            return Ok(res);
        }

        [HttpDelete("Logout")]
        [Authorize]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return NoContent();
        }
    }
}