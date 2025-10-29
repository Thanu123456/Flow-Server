using System.ComponentModel.DataAnnotations;

namespace Flow_Api.Dtos.SuperAdmin.Request
{
    public class RejectRegistrationRequestDto
    {
        [Required]
        public Guid TenantId { get; set; }

        [Required(ErrorMessage = "Rejection reason is required")]
        [StringLength(500, MinimumLength = 10)]
        public string Reason { get; set; } = string.Empty;
    }
}
