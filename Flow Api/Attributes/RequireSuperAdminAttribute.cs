using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Flow_Api.Attributes
{
    public class RequireSuperAdminAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            var isSuperAdminClaim = user.FindFirst("IsSuperAdmin")?.Value;

            if (!bool.TryParse(isSuperAdminClaim, out var isSuperAdmin) || !isSuperAdmin)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
