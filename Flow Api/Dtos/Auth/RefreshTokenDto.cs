using System.ComponentModel.DataAnnotations;

namespace Flow_Api.Dtos.Auth
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "Access token is required")]
        public string AccessToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
