using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NS.Shared.Logging.Attributes;

namespace NS.Quizzy.Server.Controllers
{
    /// <summary>
    /// Provides centralized exception handling for the application.
    /// <para>To use this controller:
    /// <list type=""></list>1. Add the following line in `Program.cs`: `app.UseExceptionHandler("/error");`
    /// 2. The middleware will redirect all unhandled exceptions to this endpoint, 
    ///    which returns a standardized error response.
    /// </para>
    /// This controller is excluded from Swagger documentation to keep the API documentation concise.
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)] // Exclude from Swagger
    [LoggingAPICallInfo]
    public class ErrorController : ControllerBase
    {
        /// <summary>
        /// Handles exceptions captured by the exception handling middleware.
        /// 
        /// This endpoint is automatically called by the `UseExceptionHandler` middleware 
        /// when an unhandled exception occurs. It provides a JSON response with the 
        /// exception message and an error flag.
        /// </summary>
        [Route("/error")]
        public IActionResult HandleError()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            var response = new
            {
                message = exception?.Message ?? "An unexpected error occurred.",
                isError = true
            };
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}
