using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using NS.Quizzy.Server.BL.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using static NS.Quizzy.Server.DAL.DALEnums;

namespace NS.Quizzy.Server.BL.CustomizationsForSwagger
{
    public class RoleRequirementOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Get the method info
            var methodInfo = (context.ApiDescription.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo;
            if (methodInfo == null)
            {
                return;
            }

            // Try to get the attribute from method or class
            var attr = methodInfo.GetCustomAttribute<RoleRequirementAttribute>()
                      ?? methodInfo.DeclaringType?.GetCustomAttribute<RoleRequirementAttribute>();

            if (attr == null)
            {
                return;
            }

            // Use reflection to access the private field _requiredRole
            var roleField = typeof(RoleRequirementAttribute)
                .GetField("_requiredRole", BindingFlags.NonPublic | BindingFlags.Instance);

            if (roleField?.GetValue(attr) is Roles requiredRole)
            {
                // Add to Swagger description
                operation.Description ??= string.Empty;
                operation.Description += $"\n\n**🛡️ Minimum role required:** {requiredRole}";

                var summary = operation.Summary ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(summary))
                {
                    summary += " ";
                }
                operation.Summary += $"🛡️{requiredRole}";
            }
        }
    }
}
