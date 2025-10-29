using Flow_Api.Data.UnitOfWork;
using Flow_Api.Models.Entities.Enums;
using Flow_Api.Models.Entities.Master;
using Flow_Api.Services.Interfaces.SuperAdmin;

namespace Flow_Api.Services.Implementations.SuperAdmin
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IMasterUnitOfWork _unitOfWork;

        public AuditLogService(IMasterUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task LogActionAsync(
            AuditActionType actionType,
            string description,
            Guid? userId = null,
            Guid? tenantId = null,
            string? ipAddress = null,
            string? oldValue = null,
            string? newValue = null)
        {
            var auditLog = new AuditLog
            {
                ActionType = actionType,
                Description = description,
                UserId = userId,
                TenantId = tenantId,
                IpAddress = ipAddress,
                OldValue = oldValue,
                NewValue = newValue,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.AuditLogs.AddAsync(auditLog);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
