using Flow_Api.Configuration;
using Flow_Api.Data;
using Flow_Api.Models.Identity;
using Flow_Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Flow_Api.Services.Implementations
{
    public class MfaService : IMfaService
    {
        private readonly ApplicationDbContext _context;
        private readonly SecuritySettings _securitySettings;

        public MfaService(
            ApplicationDbContext context,
            IOptions<SecuritySettings> securitySettings)
        {
            _context = context;
            _securitySettings = securitySettings.Value;
        }

        public async Task<string> GenerateOtpAsync(string userId, int otpType)
        {
            // Generate a random OTP
            var random = new Random();
            string otp = string.Empty;
            for (int i = 0; i < _securitySettings.OtpLength; i++)
            {
                otp += random.Next(0, 10).ToString();
            }

            // Calculate expiry date
            int expiryMinutes = otpType == 3 ? 5 : _securitySettings.OtpExpiryMinutes; // 5 minutes for 2FA, default for others
            var expiryDate = DateTime.UtcNow.AddMinutes(expiryMinutes);

            // Save OTP to database
            var userOtp = new UserOtp
            {
                UserId = userId,
                OtpCode = otp,
                OtpType = otpType,
                ExpiryDate = expiryDate,
                IsUsed = false,
                CreatedDate = DateTime.UtcNow
            };

            await _context.UserOtps.AddAsync(userOtp);
            await _context.SaveChangesAsync();

            return otp;
        }

        public async Task<bool> ValidateOtpAsync(string userId, string otp, int otpType)
        {
            // Find the latest unused OTP for the user and type
            var userOtp = await _context.UserOtps
                .Where(o => o.UserId == userId &&
                            o.OtpType == otpType &&
                            !o.IsUsed &&
                            o.ExpiryDate > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedDate)
                .FirstOrDefaultAsync();

            if (userOtp == null)
            {
                return false;
            }

            // Check if the OTP matches
            if (userOtp.OtpCode != otp)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> MarkOtpAsUsedAsync(string userId, string otp)
        {
            // Find the OTP
            var userOtp = await _context.UserOtps
                .Where(o => o.UserId == userId &&
                            o.OtpCode == otp &&
                            !o.IsUsed &&
                            o.ExpiryDate > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (userOtp == null)
            {
                return false;
            }

            // Mark as used
            userOtp.IsUsed = true;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
