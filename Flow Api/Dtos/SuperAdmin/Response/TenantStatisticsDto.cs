namespace Flow_Api.Dtos.SuperAdmin.Response
{
    public class TenantStatisticsDto
    {
        public Guid TenantId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public decimal TotalSalesLast30Days { get; set; }
        public long DatabaseSize { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public long StorageUsage { get; set; }
    }
}
