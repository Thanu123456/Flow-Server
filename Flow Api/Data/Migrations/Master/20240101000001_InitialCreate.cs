namespace Flow_Api.Data.Migrations.Master
{
    using Microsoft.EntityFrameworkCore.Migrations;
    using System;

    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "public");

            // ================================================================
            // CREATE USERS TABLE
            // ================================================================
            migrationBuilder.CreateTable(
                name: "users",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    must_change_password = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    last_password_changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    failed_login_attempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    locked_until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_super_admin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    is_mfa_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    mfa_secret = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    google_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    profile_image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    refresh_token = table.Column<string>(type: "text", nullable: true),
                    refresh_token_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_login_ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            // ================================================================
            // CREATE TENANTS TABLE
            // ================================================================
            migrationBuilder.CreateTable(
                name: "tenants",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shop_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    business_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    business_registration_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    tax_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address_line1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    address_line2 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "Sri Lanka"),
                    schema_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    registration_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    approved_by = table.Column<Guid>(type: "uuid", nullable: true),
                    approved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rejection_reason = table.Column<string>(type: "text", nullable: true),
                    rejected_by = table.Column<Guid>(type: "uuid", nullable: true),
                    rejected_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "LKR"),
                    timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Asia/Colombo"),
                    language = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "English"),
                    ip_address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    last_active_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    database_size = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    storage_usage = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenants", x => x.id);
                    table.ForeignKey(
                        name: "fk_tenants_users_owner_id",
                        column: x => x.owner_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tenants_users_approved_by",
                        column: x => x.approved_by,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tenants_users_rejected_by",
                        column: x => x.rejected_by,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            // ================================================================
            // CREATE AUDIT LOGS TABLE
            // ================================================================
            migrationBuilder.CreateTable(
                name: "audit_logs",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ip_address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    old_value = table.Column<string>(type: "text", nullable: true),
                    new_value = table.Column<string>(type: "text", nullable: true),
                    additional_data = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_audit_logs_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_audit_logs_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "public",
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            // ================================================================
            // CREATE SYSTEM SETTINGS TABLE
            // ================================================================
            migrationBuilder.CreateTable(
                name: "system_settings",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "General"),
                    is_encrypted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_settings", x => x.id);
                });

            // ================================================================
            // CREATE INDEXES
            // ================================================================

            // Users indexes
            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                schema: "public",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_tenant_id",
                schema: "public",
                table: "users",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_is_super_admin",
                schema: "public",
                table: "users",
                column: "is_super_admin");

            // Tenants indexes
            migrationBuilder.CreateIndex(
                name: "ix_tenants_schema_name",
                schema: "public",
                table: "tenants",
                column: "schema_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tenants_registration_status",
                schema: "public",
                table: "tenants",
                column: "registration_status");

            migrationBuilder.CreateIndex(
                name: "ix_tenants_owner_id",
                schema: "public",
                table: "tenants",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_tenants_approved_by",
                schema: "public",
                table: "tenants",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "ix_tenants_rejected_by",
                schema: "public",
                table: "tenants",
                column: "rejected_by");

            // Audit Logs indexes
            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_action_type",
                schema: "public",
                table: "audit_logs",
                column: "action_type");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_created_at",
                schema: "public",
                table: "audit_logs",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_user_id",
                schema: "public",
                table: "audit_logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_tenant_id",
                schema: "public",
                table: "audit_logs",
                column: "tenant_id");

            // System Settings indexes
            migrationBuilder.CreateIndex(
                name: "ix_system_settings_key",
                schema: "public",
                table: "system_settings",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_system_settings_category",
                schema: "public",
                table: "system_settings",
                column: "category");

            // ================================================================
            // ADD FOREIGN KEY FOR USER TENANT_ID
            // ================================================================
            migrationBuilder.AddForeignKey(
                name: "fk_users_tenants_tenant_id",
                schema: "public",
                table: "users",
                column: "tenant_id",
                principalSchema: "public",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys
            migrationBuilder.DropForeignKey(
                name: "fk_users_tenants_tenant_id",
                schema: "public",
                table: "users");

            // Drop tables in reverse order
            migrationBuilder.DropTable(
                name: "system_settings",
                schema: "public");

            migrationBuilder.DropTable(
                name: "audit_logs",
                schema: "public");

            migrationBuilder.DropTable(
                name: "tenants",
                schema: "public");

            migrationBuilder.DropTable(
                name: "users",
                schema: "public");
        }
    }
}