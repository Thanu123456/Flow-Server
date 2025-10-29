using System.ComponentModel.DataAnnotations;

namespace Flow_Api.Dtos.User.Request
{
    public class UpdateUserRequestDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? ProfileImageUrl { get; set; }

        public Guid? RoleId { get; set; }

        public Guid? WarehouseId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
