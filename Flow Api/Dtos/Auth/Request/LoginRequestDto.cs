using System.ComponentModel.DataAnnotations;

namespace Flow_Api.Dtos.Auth.Request
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
        public string? MfaCode { get; set; }
    }
}
