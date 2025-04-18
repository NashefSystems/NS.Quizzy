using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace NS.Quizzy.Server.BL.CustomizationsForSwagger
{
    public class AllowAnonymousOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Get the method info
            var methodInfo = (context.ApiDescription.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo;
            if (methodInfo == null)
            {
                return;
            }

            // Check for [AllowAnonymous]
            var hasAllowAnonymous =
                methodInfo.GetCustomAttribute<AllowAnonymousAttribute>() != null ||
                methodInfo.DeclaringType?.GetCustomAttribute<AllowAnonymousAttribute>() != null;

            if (!hasAllowAnonymous)
            {
                return;
            }


            // Add to Swagger description
            operation.Description ??= string.Empty;
            operation.Description += $"\n\n🔓 **This endpoint allows anonymous access.**";

            var summary = operation.Summary ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(summary))
            {
                summary += " ";
            }
            operation.Summary += "🔓";
        }
    }
}
