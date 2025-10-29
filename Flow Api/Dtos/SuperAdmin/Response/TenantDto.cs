using Flow_Api.Models.Entities.Enums;

namespace Flow_Api.Dtos.SuperAdmin.Response
{
    public class TenantDto
    {
        public Guid Id { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SchemaName { get; set; } = string.Empty;
        public RegistrationStatus Status { get; set; }
        public DateTime RegisteredDate { get; set; }
        public DateTime? LastActive { get; set; }
    }
}
