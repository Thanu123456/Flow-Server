using Flow_Api.Services.Interfaces.Auth;
using System.Text.RegularExpressions;
using BCrypt.Net;



namespace Flow_Api.Services.Implementations.Auth
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            var hasUpperCase = new Regex(@"[A-Z]").IsMatch(password);
            var hasLowerCase = new Regex(@"[a-z]").IsMatch(password);
            var hasDigit = new Regex(@"\d").IsMatch(password);
            var hasSpecialChar = new Regex(@"[@$!%*?&]").IsMatch(password);

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }
    }
}
