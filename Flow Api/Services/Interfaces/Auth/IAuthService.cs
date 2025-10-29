using Flow_Api.Dtos.Auth;
using Flow_Api.Dtos.Auth.Request;
using Flow_Api.Dtos.Auth.Response;
using System.Threading.Tasks;

namespace Flow_Api.Services.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string ipAddress);
        Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request, string ipAddress);
        Task LogoutAsync(Guid userId);
        Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
    }
}
