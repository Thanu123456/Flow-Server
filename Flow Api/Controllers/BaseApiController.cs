using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Flow_Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    // REMOVED [Authorize] from here to avoid conflicts
    public abstract class BaseApiController : ControllerBase
    {
        protected Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User ID not found");
            return Guid.Parse(userIdClaim);
        }

        protected string GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new UnauthorizedAccessException("User email not found");
        }

        protected bool IsSuperAdmin()
        {
            var isSuperAdminClaim = User.FindFirst("IsSuperAdmin")?.Value;
            return bool.TryParse(isSuperAdminClaim, out var isSuperAdmin) && isSuperAdmin;
        }

        protected Guid? GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;
            return Guid.TryParse(tenantIdClaim, out var tenantId) ? tenantId : null;
        }

        protected string GetIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}
