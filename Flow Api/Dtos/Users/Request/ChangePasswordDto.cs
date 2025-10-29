using System.ComponentModel.DataAnnotations;

namespace Flow_Api.Dtos.Users.Request
{
    public class ChangePasswordDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
