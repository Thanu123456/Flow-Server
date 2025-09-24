using System.Threading.Tasks;

namespace Flow_Api.Services.Interfaces
{
    public interface IMfaService
    {
        Task<string> GenerateOtpAsync(string userId, int otpType);
        Task<bool> ValidateOtpAsync(string userId, string otp, int otpType);
        Task<bool> MarkOtpAsUsedAsync(string userId, string otp);
    }
}
