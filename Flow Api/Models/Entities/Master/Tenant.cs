using Flow_Api.Models.Entities.Common;
using Flow_Api.Models.Entities.Enums;

namespace Flow_Api.Models.Entities.Master
{
    public class Tenant : BaseEntity
    {
        // Business Information
        public string ShopName { get; set; } = string.Empty;
        public string BusinessType { get; set; } = string.Empty;
        public string? BusinessRegistrationNumber { get; set; }
        public string? TaxNumber { get; set; }

        // Address
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public string Country { get; set; } = "Sri Lanka";

        // Schema Information
        public string? SchemaName { get; set; }
        public RegistrationStatus RegistrationStatus { get; set; } = RegistrationStatus.Pending;

        // Approval Information
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }
        public Guid? RejectedBy { get; set; }
        public DateTime? RejectedAt { get; set; }

        // Settings
        public string Currency { get; set; } = "LKR";
        public string Timezone { get; set; } = "Asia/Colombo";
        public string Language { get; set; } = "English";

        // Metadata
        public string? IpAddress { get; set; }
        public DateTime? LastActiveAt { get; set; }
        public long DatabaseSize { get; set; } = 0;
        public long StorageUsage { get; set; } = 0;

        // Navigation Properties
        public Guid OwnerId { get; set; }
        public User Owner { get; set; } = null!;
    }
}
