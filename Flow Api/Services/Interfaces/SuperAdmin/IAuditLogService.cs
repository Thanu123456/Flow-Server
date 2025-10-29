using Flow_Api.Models.Entities.Enums;

namespace Flow_Api.Services.Interfaces.SuperAdmin
{
    public interface IAuditLogService
    {
        Task LogActionAsync(AuditActionType actionType, string description, Guid? userId = null,
            Guid? tenantId = null, string? ipAddress = null, string? oldValue = null, string? newValue = null);
    }
}
