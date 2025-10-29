using Flow_Api.Models.Entities.Common;
using Flow_Api.Models.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Flow_Api.Models.Entities.Master
{
    public class User : BaseEntity
    {
        // Personal Information
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // Authentication
        public string PasswordHash { get; set; } = string.Empty;
        public bool MustChangePassword { get; set; } = false;
        public DateTime? LastPasswordChangedAt { get; set; }
        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockedUntil { get; set; }

        // Role Information
        public bool IsSuperAdmin { get; set; } = false;
        public Guid? TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        // Status
        public UserStatus Status { get; set; } = UserStatus.Active;

        // MFA
        public bool IsMfaEnabled { get; set; } = false;
        public string? MfaSecret { get; set; }

        // Google OAuth
        public string? GoogleId { get; set; }
        public string? ProfileImageUrl { get; set; }

        // Refresh Tokens
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }

        // Activity
        public DateTime? LastLoginAt { get; set; }
        public string? LastLoginIp { get; set; }
    }
}
