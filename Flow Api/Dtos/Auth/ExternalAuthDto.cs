using System.ComponentModel.DataAnnotations;

namespace Flow_Api.Dtos.Auth
{
    public class ExternalAuthDto
    {
        [Required(ErrorMessage = "Provider is required")]
        public string Provider { get; set; } = string.Empty;

        [Required(ErrorMessage = "Id token is required")]
        public string IdToken { get; set; } = string.Empty;
    }
}
