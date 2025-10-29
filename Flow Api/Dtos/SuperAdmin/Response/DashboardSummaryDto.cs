namespace Flow_Api.Dtos.SuperAdmin.Response
{
    public class DashboardSummaryDto
    {
        public int PendingRegistrations { get; set; }
        public int TotalTenants { get; set; }
        public int ActiveUsers { get; set; }
        public SystemHealthDto SystemHealth { get; set; } = new();
    }
}
