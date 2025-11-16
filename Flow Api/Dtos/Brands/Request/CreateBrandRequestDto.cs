using System.ComponentModel.DataAnnotations;

namespace Flow_Api.Dtos.Brands.Request
{
    public class CreateBrandRequestDto
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ImageBase64 { get; set; }

        public string Status { get; set; } = "active";
    }

}
