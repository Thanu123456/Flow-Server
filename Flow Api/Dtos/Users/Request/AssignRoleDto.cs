using System.ComponentModel.DataAnnotations;

namespace Flow_Api.Dtos.Users.Request
{
    public class AssignRoleDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public Guid RoleId { get; set; }
    }
}
