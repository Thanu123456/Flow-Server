using Flow_Api.Models.Entities.Tenant.Management;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flow_Api.Data.Configurations
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("brands");

            // Primary Key
            builder.HasKey(b => b.Id)
                   .HasName("pk_brands");

            builder.Property(b => b.Id)
                   .HasColumnType("uuid")
                   .HasDefaultValueSql("uuid_generate_v4()")
                   .ValueGeneratedOnAdd();

            // Name
            builder.Property(b => b.Name)
                   .IsRequired()
                   .HasMaxLength(255)
                   .HasColumnType("varchar(255)");

            // Description
            builder.Property(b => b.Description)
                   .IsRequired(false)
                   .HasMaxLength(500)
                   .HasColumnType("varchar(500)");

            // ImageBase64 (can be large – use text)
            builder.Property(b => b.ImageBase64)
                   .IsRequired(false)
                   .HasColumnType("text");

            // Status
            builder.Property(b => b.Status)
                   .IsRequired()
                   .HasMaxLength(20)
                   .HasColumnType("varchar(20)")
                   .HasDefaultValue("active");

            // Timestamps
            builder.Property(b => b.CreatedAt)
                   .IsRequired()
                   .HasColumnType("timestamp with time zone")
                   .HasDefaultValueSql("now()");

            builder.Property(b => b.UpdatedAt)
                   .IsRequired(false)
                   .HasColumnType("timestamp with time zone");

            // Indexes
            builder.HasIndex(b => b.Name)
                   .HasDatabaseName("ix_brands_name");

            builder.HasIndex(b => b.Status)
                   .HasDatabaseName("ix_brands_status");

            // Optional: unique constraint if you want DB-level enforcement
            // builder.HasIndex(b => b.Name).IsUnique();
        }
    }
}