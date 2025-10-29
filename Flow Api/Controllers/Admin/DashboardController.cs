using Flow_Api.Attributes;
using Flow_Api.Data.UnitOfWork;
using Flow_Api.Dtos.Common;
using Flow_Api.Dtos.SuperAdmin.Response;
using Flow_Api.Models.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace Flow_Api.Controllers.Admin
{
    [Authorize]
    [RequireSuperAdmin]
    public class DashboardController : BaseApiController
    {
        private readonly IMasterUnitOfWork _unitOfWork;

        public DashboardController(IMasterUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> GetDashboardSummary()
        {
            var pendingCount = await _unitOfWork.Tenants.CountAsync(
                t => t.RegistrationStatus == RegistrationStatus.Pending);

            var totalTenants = await _unitOfWork.Tenants.CountAsync(
                t => t.RegistrationStatus == RegistrationStatus.Active);

            var totalUsers = await _unitOfWork.Users.CountAsync();

            var summary = new DashboardSummaryDto
            {
                PendingRegistrations = pendingCount,
                TotalTenants = totalTenants,
                ActiveUsers = totalUsers,
                SystemHealth = new SystemHealthDto
                {
                    DatabaseHealthy = true,
                    ApiUptime = 99.9,
                    LastBackup = DateTime.UtcNow.AddHours(-12),
                    Status = "Healthy"
                }
            };

            return Ok(ApiResponse<DashboardSummaryDto>.SuccessResponse(
                summary,
                "Dashboard summary retrieved"
            ));
        }
    }
}
