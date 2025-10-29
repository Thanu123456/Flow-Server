using Flow_Api.Attributes;
using Flow_Api.Data.UnitOfWork;
using Flow_Api.Dtos.Common;
using Flow_Api.Models.Entities.Enums;
using Flow_Api.Models.Entities.Master;
using Flow_Api.Services.Interfaces.SuperAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace Flow_Api.Controllers.SuperAdmin
{
    [Authorize]
    [RequireSuperAdmin]
    public class SystemSettingsController : BaseApiController
    {
        private readonly IMasterUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;

        public SystemSettingsController(
            IMasterUnitOfWork unitOfWork,
            IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<SystemSetting>>>> GetAllSettings()
        {
            var settings = await _unitOfWork.SystemSettings.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<SystemSetting>>.SuccessResponse(
                settings,
                "Settings retrieved"
            ));
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SystemSetting>>>> GetByCategory(string category)
        {
            var settings = await _unitOfWork.SystemSettings.GetByCategoryAsync(category);
            return Ok(ApiResponse<IEnumerable<SystemSetting>>.SuccessResponse(
                settings,
                "Settings retrieved"
            ));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SystemSetting>>> CreateSetting(
            [FromBody] SystemSetting setting)
        {
            await _unitOfWork.SystemSettings.AddAsync(setting);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                AuditActionType.SettingsUpdated,
                $"System setting created: {setting.Key}",
                GetCurrentUserId(),
                null,
                GetIpAddress()
            );

            return Ok(ApiResponse<SystemSetting>.SuccessResponse(
                setting,
                "Setting created"
            ));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<SystemSetting>>> UpdateSetting(
            Guid id,
            [FromBody] SystemSetting setting)
        {
            var existingSetting = await _unitOfWork.SystemSettings.GetByIdAsync(id);

            if (existingSetting == null)
                return NotFound(ApiResponse<SystemSetting>.ErrorResponse("Setting not found"));

            var oldValue = existingSetting.Value;
            existingSetting.Value = setting.Value;
            existingSetting.Description = setting.Description;
            existingSetting.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                AuditActionType.SettingsUpdated,
                $"System setting updated: {existingSetting.Key}",
                GetCurrentUserId(),
                null,
                GetIpAddress(),
                oldValue,
                setting.Value
            );

            return Ok(ApiResponse<SystemSetting>.SuccessResponse(
                existingSetting,
                "Setting updated"
            ));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteSetting(Guid id)
        {
            var setting = await _unitOfWork.SystemSettings.GetByIdAsync(id);

            if (setting == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Setting not found"));

            _unitOfWork.SystemSettings.Remove(setting);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                AuditActionType.SettingsUpdated,
                $"System setting deleted: {setting.Key}",
                GetCurrentUserId(),
                null,
                GetIpAddress()
            );

            return Ok(ApiResponse<object>.SuccessResponse(null, "Setting deleted"));
        }
    }
}
