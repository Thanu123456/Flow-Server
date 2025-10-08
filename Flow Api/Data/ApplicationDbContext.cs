using Flow_Api.Models.Entities;
using Flow_Api.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Flow_Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<
    ApplicationUser,
    ApplicationRole,
    string,
    IdentityUserClaim<string>,
    IdentityUserRole<string>, // <-- Fix: Use IdentityUserRole<string> here
    IdentityUserLogin<string>,
    ApplicationRoleClaim,     // <-- Fix: Use ApplicationRoleClaim here
    IdentityUserToken<string>>
    {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
            {
            }

            public DbSet<Permission> Permissions { get; set; }
            public DbSet<UserOtp> UserOtps { get; set; }
            public DbSet<RefreshToken> RefreshTokens { get; set; }

        // In your Data/ApplicationDbContext.cs file

       protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    // Configure ApplicationUser
    builder.Entity<ApplicationUser>(entity =>
    {
        entity.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
        entity.Property(u => u.LastName).HasMaxLength(50).IsRequired();
        entity.Property(u => u.ProfileImageUrl).HasMaxLength(255).IsRequired(false);
        entity.Property(u => u.IsActive).HasDefaultValue(true);
        entity.Property(u => u.CreatedDate).HasDefaultValueSql("NOW()");
    });

    // Configure ApplicationRole
    builder.Entity<ApplicationRole>(entity =>
    {
        entity.Property(r => r.Description).HasMaxLength(255);
    });

    // Configure Permission
    builder.Entity<Permission>(entity =>
    {
        entity.HasKey(p => p.Id);
        entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
        entity.Property(p => p.Description).HasMaxLength(255);
        entity.Property(p => p.Category).IsRequired().HasMaxLength(50);
        entity.Property(p => p.CreatedDate).HasDefaultValueSql("NOW()");
    });

    // Configure UserOtp
    builder.Entity<UserOtp>(entity =>
    {
        entity.HasKey(o => o.Id);
        entity.Property(o => o.UserId).IsRequired(false); // Explicitly set as not required
        entity.Property(o => o.OtpCode).IsRequired().HasMaxLength(6);
        entity.Property(o => o.OtpType).IsRequired();
        entity.Property(o => o.ExpiryDate).IsRequired();
        entity.Property(o => o.IsUsed).IsRequired().HasDefaultValue(false);
        entity.Property(o => o.CreatedDate).HasDefaultValueSql("NOW()");
        entity.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .IsRequired(false) // Explicitly set as not required
            .OnDelete(DeleteBehavior.Cascade);
    });

    // Configure RefreshToken
    builder.Entity<RefreshToken>(entity =>
    {
        entity.HasKey(r => r.Id);
        entity.Property(r => r.UserId).IsRequired(false); // Explicitly set as not required
        entity.Property(r => r.Token).IsRequired();
        entity.Property(r => r.JwtId).IsRequired();
        entity.Property(r => r.IsUsed).IsRequired().HasDefaultValue(false);
        entity.Property(r => r.IsRevoked).IsRequired().HasDefaultValue(false);
        entity.Property(r => r.IssuedAt).IsRequired();
        entity.Property(r => r.ExpiresAt).IsRequired();
        entity.Property(r => r.CreatedDate).HasDefaultValueSql("NOW()");
        entity.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .IsRequired(false) // Explicitly set as not required
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasIndex(r => r.Token).IsUnique();
    });

    // Configure RolePermissions (many-to-many relationship)
    builder.Entity<ApplicationRole>()
        .HasMany<ApplicationRoleClaim>()
        .WithOne()
        .HasForeignKey(rc => rc.RoleId)
        .IsRequired();
}
    }

        // UserOtp entity
        public class UserOtp
        {
            public Guid Id { get; set; }
            public string? UserId { get; set; } // Made nullable to satisfy CS8618
            public required string OtpCode { get; set; }
            public int OtpType { get; set; } // 1: Email Verification, 2: Password Reset, 3: 2FA
            public DateTime ExpiryDate { get; set; }
            public bool IsUsed { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        // RefreshToken entity
        public class RefreshToken
        {
            public Guid Id { get; set; }
            public string? UserId { get; set; } // Made nullable to satisfy CS8618
            public required string Token { get; set; }
            public required string JwtId { get; set; }
            public bool IsUsed { get; set; }
            public bool IsRevoked { get; set; }
            public DateTime IssuedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
            public DateTime CreatedDate { get; set; }
        }
    
}

