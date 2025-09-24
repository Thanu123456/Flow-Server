using System.ComponentModel.DataAnnotations;

namespace Flow_Api.Dtos.Auth
{
    public class VerifyOtpDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "OTP is required")]
        public required string Otp { get; set; }

        [Required(ErrorMessage = "OTP type is required")]
        public int OtpType { get; set; } // 1: Email Verification, 2: Password Reset, 3: 2FA
    }
}
