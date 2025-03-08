using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using static NS.Quizzy.Server.DAL.DALEnums;

namespace NS.Quizzy.Server.BL.Attributes
{
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
                context.Result = new UnauthorizedResult();
                return;
            }

            var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (roleClaim == null || !Enum.TryParse(roleClaim, out Roles userRole))
            {
                context.Result = new ForbidResult();
                return;
            }

            if (userRole < _requiredRole)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
