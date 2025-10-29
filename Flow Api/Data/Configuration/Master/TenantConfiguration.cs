using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flow_Api.Models.Entities.Master;
using Flow_Api.Models.Entities.Enums;

namespace Flow_Api.Data.Configuration.Master
{
    public class TenantConfiguration : IEntityTypeConfiguration<Flow_Api.Models.Entities.Master.Tenant>
    {
        public void Configure(EntityTypeBuilder<Flow_Api.Models.Entities.Master.Tenant> builder)
        {
            builder.ToTable("tenants", "public");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.ShopName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.BusinessType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.SchemaName)
                .HasMaxLength(100);

            builder.Property(t => t.Currency)
                .HasMaxLength(10);

            builder.HasOne(t => t.Owner)
                .WithOne()
                .HasForeignKey<Flow_Api.Models.Entities.Master.Tenant>(t => t.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(t => t.SchemaName).IsUnique();
            builder.HasIndex(t => t.RegistrationStatus);
        }
    }
}
