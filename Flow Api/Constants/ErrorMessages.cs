namespace Flow_Api.Constants
{
    public static class ErrorMessages
    {
        public const string UnauthorizedAccess = "You do not have permission to perform this action";
        public const string InvalidCredentials = "Invalid email or password";
        public const string AccountLocked = "Account is locked due to too many failed attempts";
        public const string AccountNotActive = "Account is not active";
        public const string EmailAlreadyExists = "Email already registered";
        public const string TenantNotFound = "Tenant not found";
        public const string UserNotFound = "User not found";
        public const string InvalidToken = "Invalid or expired token";
    }
}
