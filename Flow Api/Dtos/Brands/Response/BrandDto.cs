namespace Flow_Api.Dtos.Brands.Response
{
    public class BrandDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ImageBase64 { get; set; }

        public string Status { get; set; } = "active";

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
