using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NS.Quizzy.Server.BL.CustomExceptions;
using System.Security.Claims;
using static NS.Quizzy.Server.DAL.DALEnums;

namespace NS.Quizzy.Server.BL.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RoleRequirementAttribute : Attribute, IAuthorizationFilter
    {
        private readonly Roles _requiredRole;

        public RoleRequirementAttribute(Roles requiredRole = Roles.Student)
        {
            _requiredRole = requiredRole;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                throw new UnauthorizedException("Invalid credentials");
            }

            var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (roleClaim == null || !Enum.TryParse(roleClaim, out Roles userRole))
            {
                throw new ForbiddenException("You do not have permission to perform this action");
            }

            if (userRole < _requiredRole)
            {
                throw new ForbiddenException("You do not have permission to perform this action");
            }
        }
    }
}
