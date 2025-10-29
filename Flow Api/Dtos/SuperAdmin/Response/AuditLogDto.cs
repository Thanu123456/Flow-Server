using Flow_Api.Models.Entities.Enums;

namespace Flow_Api.Dtos.SuperAdmin.Response
{
    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public AuditActionType ActionType { get; set; }
        public string Description { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? TenantName { get; set; }
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
