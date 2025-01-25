using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.Models;
using NS.Shared.Logging.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace NS.Quizzy.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, null, typeof(GlobalErrorResponse))]
    [LoggingAPICallInfo]
    public class ClientAppSettingsController : ControllerBase
    {
        private readonly IAppSettingsService _appSettingsService;

        public ClientAppSettingsController(IAppSettingsService appSettingsService)
        {
            _appSettingsService = appSettingsService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Dictionary<string, object?>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Dictionary<string, object?>>> GetAsync()
        {
            var res = await _appSettingsService.GetByTargetAsync(DAL.DALEnums.AppSettingTargets.Client);
            return Ok(res);
        }
    }
}