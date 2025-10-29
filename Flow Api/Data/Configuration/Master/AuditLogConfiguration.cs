using Flow_Api.Models.Entities.Master;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flow_Api.Data.Configuration.Master
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("audit_logs", "public");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(a => a.Tenant)
                .WithMany()
                .HasForeignKey(a => a.TenantId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(a => a.ActionType);
            builder.HasIndex(a => a.CreatedAt);
        }
    }
}
