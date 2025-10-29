namespace Flow_Api.Models.Requests
{
    public class SearchRequest
    {
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "asc";
        public Guid? RoleId { get; set; }
        public Guid? WarehouseId { get; set; }
        public bool? IsActive { get; set; }
    }
}
