using Flow_Api.Attributes;
using Flow_Api.Dtos.Common;
using Flow_Api.Dtos.SuperAdmin.Request;
using Flow_Api.Services.Interfaces.SuperAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flow_Api.Controllers.SuperAdmin
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    [RequireSuperAdmin]
    public class RegistrationApprovalController : BaseApiController
    {
        private readonly IRegistrationApprovalService _approvalService;

        public RegistrationApprovalController(IRegistrationApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        [HttpGet("pending")]
        public async Task<ActionResult<ApiResponse<object>>> GetPendingRegistrations()
        {
            var result = await _approvalService.GetPendingRegistrationsAsync();
            return Ok(ApiResponse<object>.SuccessResponse(result, "Pending registrations retrieved"));
        }

        [HttpPost("approve")]
        public async Task<ActionResult<ApiResponse<object>>> ApproveRegistration(
            [FromBody] ApproveRegistrationRequestDto request)
        {
            var superAdminId = GetCurrentUserId();
            await _approvalService.ApproveRegistrationAsync(request, superAdminId);
            return Ok(ApiResponse<object>.SuccessResponse(null!, "Registration approved successfully"));
        }

        [HttpPost("reject")]
        public async Task<ActionResult<ApiResponse<object>>> RejectRegistration(
            [FromBody] RejectRegistrationRequestDto request)
        {
            var superAdminId = GetCurrentUserId();
            await _approvalService.RejectRegistrationAsync(request, superAdminId);
            return Ok(ApiResponse<object>.SuccessResponse(null!, "Registration rejected"));
        }
    }
}