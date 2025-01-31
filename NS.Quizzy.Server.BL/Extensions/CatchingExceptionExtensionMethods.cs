using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NS.Quizzy.Server.BL.CustomExceptions;
using NS.Quizzy.Server.BL.Middlewares;

namespace NS.Quizzy.Server.BL.Extensions
{
    public static class CatchingExceptionExtensionMethods
    {
        public static IApplicationBuilder UseCatchingExceptionMiddleware(this IApplicationBuilder app)
        {
            app?.UseMiddleware<CatchingExceptionMiddleware>();
            return app;
        }

        public static IMvcBuilder CustomConfigureApiBehaviorOptions(this IMvcBuilder builder)
        {
            builder.ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = GetModelStateErrorWithException(context);
                    throw new BadRequestException("Validation error:\n" + string.Join(", ", errors));
                };
            });
            return builder;
        }

        private static List<string> GetModelStateErrorWithException(ActionContext actionContext)
        {
            var res = new List<string>();
            foreach (var error in actionContext.ModelState)
            {
                string message = string.Empty;
                if (error.Value.Errors != null && error.Value.Errors.Count() > 0)
                {
                    if (error.Value.Errors[0].Exception != null)
                    {
                        // More weight is given to the exception message
                        message = error.Value.Errors[0].Exception.Message;
                    }
                    else if (!string.IsNullOrEmpty(error.Value.Errors[0].ErrorMessage))
                    {
                        // Otherwise, use the error message
                        message = error.Value.Errors[0].ErrorMessage;
                    }
                }

                if (!string.IsNullOrWhiteSpace(message))
                {
                    if (!message.Contains(error.Key))
                    {
                        res.Add($"{error.Key}: {message}");
                    }
                    else
                    {
                        res.Add(message);
                    }
                }
            }
            return res;
        }
    }
}
