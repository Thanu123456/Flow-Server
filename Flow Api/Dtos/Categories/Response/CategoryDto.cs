namespace Flow_Api.Dtos.Categories.Response
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Code { get; set; }
        public bool Status { get; set; }
    }
}
