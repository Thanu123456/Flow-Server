using Flow_Api.Models.Entities.Common;

namespace Flow_Api.Models.Entities.Master
{
    public class SystemSetting : BaseEntity
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = "General";
        public bool IsEncrypted { get; set; } = false;
    }
}
