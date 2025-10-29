namespace Flow_Api.Models.Entities.Enums
{
    public enum AuditActionType
    {
        RegistrationSubmitted,
        RegistrationApproved,
        RegistrationRejected,
        TenantSuspended,
        TenantDeleted,
        SuperAdminLogin,
        SchemaCreated,
        MigrationExecuted,
        UserCreated,
        UserUpdated,
        UserDeleted,
        SettingsUpdated
    }
}
