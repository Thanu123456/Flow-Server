using System.ComponentModel.DataAnnotations;


namespace Flow_Api.Dtos.Brands.Request
{
    public class UpdateBrandRequestDto
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = "active";

        public string? ImageBase64 { get; set; }

    }
}
