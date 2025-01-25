using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using NS.Shared.Logging.Extensions;
using NS.Quizzy.Server.Models.Models;
using NS.Quizzy.Server.BL.CustomExceptions;

namespace NS.Quizzy.Server.BL.Middlewares
{
    internal class CatchingExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public CatchingExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            Exception? exception = null; int statusCode;
            try
            {
                await _next(context);
                statusCode = context.Response.StatusCode;
            }
            catch (Exception _ex)
            {
                exception = _ex;
                statusCode = StatusCodes.Status500InternalServerError;
            }

            if (!context.Request.Path.ToString().StartsWith("/api", StringComparison.CurrentCultureIgnoreCase) || statusCode < StatusCodes.Status400BadRequest)
            {
                return;
            }

            var logger = serviceProvider.GetNSLogger();
            GlobalErrorResponse response = new() { ContextId = logger.GetContextId() };
            exception ??= context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (exception != null && exception is Exception ex)
            {
                if (ex is BaseCustomException baseCustomException)
                {
                    statusCode = baseCustomException.StatusCode;
                    response.Message = baseCustomException.Message;
                }
                else
                {
                    statusCode = StatusCodes.Status500InternalServerError;
                    response.Message = ex.Message;
                }
            }
            else
            {
                if (statusCode == StatusCodes.Status401Unauthorized)
                {
                    response.Message = "invalid token";
                }
                else
                {
                    response.Message = await context.Response.FormatResponse();
                }
            }

            if (statusCode < StatusCodes.Status500InternalServerError)
            {
                logger.Fatal($"CatchingException ({response.Message})", new { StatusCode = statusCode, ErrorMessage = response.Message }, exception);
            }
            else
            {
                logger.Error($"CatchingException ({response.Message})", new { StatusCode = statusCode, ErrorMessage = response.Message }, exception);
            }

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response);
            context.Response.Body.Position = 0;
        }
    }
}