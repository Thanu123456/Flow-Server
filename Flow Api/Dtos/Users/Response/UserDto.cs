namespace Flow_Api.Dtos.User.Response
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public bool IsSuperAdmin { get; set; }
        public Guid? TenantId { get; set; }
        public string? TenantName { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }
        public Guid? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
