using System.Collections.Generic;
using Flow_Api.Models.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Flow_Api.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(ApplicationUser user, IList<string> roles, IList<string> permissions);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<string> SaveRefreshTokenAsync(string userId, string refreshToken, string jwtId);
        Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string userId, string refreshToken);
    }
}
