namespace Flow_Api.Dtos.Auth.Response
{
    public class UserInfoResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsSuperAdmin { get; set; }
        public Guid? TenantId { get; set; }
        public string? TenantName { get; set; }
        public string? SchemaName { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
