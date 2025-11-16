namespace Flow_Api.Dtos.Categories.Request
{
    public class CreateCategoryRequestDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
    }
}
