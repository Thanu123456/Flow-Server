using Flow_Api.Dtos.Auth;
using Flow_Api.Dtos.Auth.Request;
using Flow_Api.Dtos.Common;
using Flow_Api.Models.Entities;
using Flow_Api.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flow_Api.Controllers.Public
{
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous] // Only this endpoint allows anonymous access
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<object>>> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request, GetIpAddress());
            return Ok(ApiResponse<object>.SuccessResponse(result, "Login successful"));
        }

        [AllowAnonymous] // Only this endpoint allows anonymous access
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<object>>> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request, GetIpAddress());
            return Ok(ApiResponse<object>.SuccessResponse(
                result,
                "Registration submitted successfully. Please wait for admin approval."
            ));
        }

        [Authorize] // Only this endpoint requires auth
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            await _authService.LogoutAsync(GetCurrentUserId());
            return Ok(ApiResponse<object>.SuccessResponse(null!, "Logout successful"));
        }

        [AllowAnonymous] // Only this endpoint allows anonymous access
        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<object>>> RefreshToken([FromBody] string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Token refreshed"));
        }
    }
}