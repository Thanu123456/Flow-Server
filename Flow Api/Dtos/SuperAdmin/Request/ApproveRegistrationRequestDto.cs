using System.ComponentModel.DataAnnotations;

namespace Flow_Api.Dtos.SuperAdmin.Request
{
    public class ApproveRegistrationRequestDto
    {
        [Required]
        public Guid TenantId { get; set; }
    }
}
