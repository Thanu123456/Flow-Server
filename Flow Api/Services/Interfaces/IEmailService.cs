using System.Threading.Tasks;

namespace Flow_Api.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
        Task SendOtpEmailAsync(string email, string otp, int otpType);
    }
}
