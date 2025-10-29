using System.ComponentModel.DataAnnotations;

namespace Flow_Api.Dtos.SuperAdmin.Request
{
    public class SuspendTenantRequestDto
    {
        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}
