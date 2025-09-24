using Flow_Api.Dtos.Auth;
using System.Threading.Tasks;

namespace Flow_Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<TokenDto> LoginAsync(LoginDto loginDto);
        Task<TokenDto> RegisterAsync(RegisterDto registerDto);
        Task<TokenDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<bool> LogoutAsync(string userId);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto);
        Task<TokenDto> ExternalLoginAsync(ExternalAuthDto externalAuthDto);
        Task<bool> SendOtpAsync(string email, int otpType);
    }
}
