namespace Flow_Api.Dtos.SuperAdmin.Response
{
    public class SystemHealthDto
    {
        public bool DatabaseHealthy { get; set; }
        public double ApiUptime { get; set; }
        public DateTime? LastBackup { get; set; }
        public string Status { get; set; } = "Healthy";
    }
}
