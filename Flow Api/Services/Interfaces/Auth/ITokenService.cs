using Flow_Api.Models.Entities.Master;
using System.Security.Claims;

namespace Flow_Api.Services.Interfaces.Auth
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
