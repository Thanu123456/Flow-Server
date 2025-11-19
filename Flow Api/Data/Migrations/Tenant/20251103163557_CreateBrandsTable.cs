using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flow_Api.Data.Migrations.Tenant
{
    /// <inheritdoc />
    public partial class CreateBrandsTable : Migration
    {
        /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Enable UUID extension FIRST
        migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");
        
        // Then create the brands table
        migrationBuilder.CreateTable(
            name: "brands",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                ImageBase64 = table.Column<string>(type: "text", nullable: true),
                Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "active"),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_brands", x => x.Id);
            });
    }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
