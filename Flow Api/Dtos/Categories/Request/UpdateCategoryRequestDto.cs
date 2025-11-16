namespace Flow_Api.Dtos.Categories.Request
{
    public class UpdateCategoryRequestDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
    }
}
