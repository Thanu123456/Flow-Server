namespace Flow_Api.Configuration
{
    public class SecuritySettings
    {
        public int PasswordRequireDigit { get; set; }
        public int PasswordRequiredLength { get; set; }
        public int PasswordRequireNonAlphanumeric { get; set; }
        public int PasswordRequireUppercase { get; set; }
        public int PasswordRequireLowercase { get; set; }
        public int MaxFailedAccessAttempts { get; set; }
        public int DefaultLockoutDurationInMinutes { get; set; }
        public int OtpExpiryMinutes { get; set; }
        public int OtpLength { get; set; }
    }
}
