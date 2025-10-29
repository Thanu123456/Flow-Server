using Flow_Api.Models.Entities.Common;
using Flow_Api.Models.Entities.Enums;

namespace Flow_Api.Models.Entities.Master
{
    public class AuditLog : BaseEntity
    {
        public AuditActionType ActionType { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public Guid? TenantId { get; set; }
        public Tenant? Tenant { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? AdditionalData { get; set; }
    }
}
