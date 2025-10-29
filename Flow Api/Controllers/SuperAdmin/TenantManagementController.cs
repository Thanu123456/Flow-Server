using AutoMapper;
using Flow_Api.Attributes;
using Flow_Api.Data.UnitOfWork;
using Flow_Api.Dtos.Common;
using Flow_Api.Dtos.SuperAdmin.Request;
using Flow_Api.Dtos.SuperAdmin.Response;
using Flow_Api.Models.Entities.Enums;
using Flow_Api.Services.Interfaces.SuperAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenantEntity = Flow_Api.Models.Entities.Master.Tenant;

namespace Flow_Api.Controllers.SuperAdmin
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    [RequireSuperAdmin]
    public class TenantManagementController : BaseApiController
    {
        private readonly IMasterUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        public TenantManagementController(
            IMasterUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TenantDto>>>> GetActiveTenants()
        {
            var tenants = await _unitOfWork.Tenants.FindAsync(t =>
                t.RegistrationStatus == RegistrationStatus.Active);

            var tenantDtos = _mapper.Map<IEnumerable<TenantDto>>(tenants);

            return Ok(ApiResponse<IEnumerable<TenantDto>>.SuccessResponse(
                tenantDtos,
                "Active tenants retrieved"
            ));
        }

        [HttpGet("all")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TenantDto>>>> GetAllTenants()
        {
            var tenants = await _unitOfWork.Tenants.GetAllAsync();
            var tenantDtos = _mapper.Map<IEnumerable<TenantDto>>(tenants);

            return Ok(ApiResponse<IEnumerable<TenantDto>>.SuccessResponse(
                tenantDtos,
                "All tenants retrieved"
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TenantDto>>> GetTenantById(Guid id)
        {
            var tenantById = await _unitOfWork.Tenants.GetByIdWithOwnerAsync(id);

            if (tenantById == null)
                return NotFound(ApiResponse<TenantDto>.ErrorResponse("Tenant not found"));

            var tenantDto = _mapper.Map<TenantDto>(tenantById);

            return Ok(ApiResponse<TenantDto>.SuccessResponse(tenantDto, "Tenant retrieved"));
        }

        [HttpPost("suspend")]
        public async Task<ActionResult<ApiResponse<object>>> SuspendTenant(
            [FromBody] SuspendTenantRequestDto request)
        {
            var suspendTenant = await _unitOfWork.Tenants.GetByIdAsync(request.TenantId);

            if (suspendTenant == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Tenant not found"));

            suspendTenant.RegistrationStatus = RegistrationStatus.Suspended;
            suspendTenant.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                AuditActionType.TenantSuspended,
                $"Tenant {suspendTenant.ShopName} suspended: {request.Reason}",
                GetCurrentUserId(),
                suspendTenant.Id,
                GetIpAddress()
            );

            return Ok(ApiResponse<object>.SuccessResponse(null, "Tenant suspended successfully"));
        }

        [HttpPost("activate")]
        public async Task<ActionResult<ApiResponse<object>>> ActivateTenant([FromBody] Guid tenantId)
        {
            var activateTenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);

            if (activateTenant == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Tenant not found"));

            activateTenant.RegistrationStatus = RegistrationStatus.Active;
            activateTenant.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                AuditActionType.RegistrationApproved,
                $"Tenant {activateTenant.ShopName} activated",
                GetCurrentUserId(),
                activateTenant.Id,
                GetIpAddress()
            );

            return Ok(ApiResponse<object>.SuccessResponse(null, "Tenant activated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteTenant(Guid id, [FromBody] string confirmPassword)
        {
            var deleteTenant = await _unitOfWork.Tenants.GetByIdAsync(id);

            if (deleteTenant == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Tenant not found"));

            // Soft delete
            deleteTenant.IsDeleted = true;
            deleteTenant.DeletedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                AuditActionType.TenantDeleted,
                $"Tenant {deleteTenant.ShopName} deleted",
                GetCurrentUserId(),
                deleteTenant.Id,
                GetIpAddress()
            );

            return Ok(ApiResponse<object>.SuccessResponse(null, "Tenant deleted successfully"));
        }

        [HttpGet("{id}/statistics")]
        public async Task<ActionResult<ApiResponse<TenantStatisticsDto>>> GetTenantStatistics(Guid id)
        {
            var statsTenant = await _unitOfWork.Tenants.GetByIdAsync(id);

            if (statsTenant == null)
                return NotFound(ApiResponse<TenantStatisticsDto>.ErrorResponse("Tenant not found"));

            // TODO: Query tenant-specific database for statistics
            var statistics = new TenantStatisticsDto
            {
                TenantId = statsTenant.Id,
                ShopName = statsTenant.ShopName,
                TotalUsers = 0, // Query from tenant schema
                TotalProducts = 0,
                TotalSalesLast30Days = 0,
                DatabaseSize = statsTenant.DatabaseSize,
                LastLoginDate = statsTenant.LastActiveAt,
                StorageUsage = statsTenant.StorageUsage
            };

            return Ok(ApiResponse<TenantStatisticsDto>.SuccessResponse(
                statistics,
                "Tenant statistics retrieved"
            ));
        }
    }
}
