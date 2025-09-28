using Flow_Api.Configuration;
using Flow_Api.Data;
using Flow_Api.Dtos.Auth;
using Flow_Api.Models.Identity;
using Flow_Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Flow_Api.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IMfaService _mfaService;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext context,
            ITokenService tokenService,
            IEmailService emailService,
            IMfaService mfaService,
            IOptions<JwtSettings> jwtSettings,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _tokenService = tokenService;
            _emailService = emailService;
            _mfaService = mfaService;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning($"Login attempt with non-existent email: {loginDto.Email}");
                throw new ArgumentException("Invalid email or password");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning($"Login attempt by inactive user: {user.Email}");
                throw new ArgumentException("Account is disabled. Please contact support.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning($"Failed login attempt for user: {user.Email}");
                throw new ArgumentException("Invalid email or password");
            }

            // Check if email is confirmed
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                _logger.LogWarning($"Login attempt by unconfirmed email: {user.Email}");
                throw new ArgumentException("Please confirm your email before logging in");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var userPermissions = await GetUserPermissionsAsync(user.Id);

            var accessToken = _tokenService.GenerateAccessToken(user, userRoles, userPermissions);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var jwtId = new JwtSecurityToken(accessToken).Id;

            await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken, jwtId);

            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                // ... inside LoginAsync, RegisterAsync, and RefreshTokenAsync where UserDto is constructed:
                User = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    ProfileImageUrl = user.ProfileImageUrl ?? string.Empty,
                    Roles = userRoles.ToList(),
                    Permissions = userPermissions.ToList()
                }
            };
        }

        public async Task<TokenDto> RegisterAsync(RegisterDto registerDto)
        {
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                _logger.LogWarning($"Registration attempt with existing email: {registerDto.Email}");
                throw new ArgumentException("Email already exists");
            }

            if (string.IsNullOrWhiteSpace(registerDto.Password))
            {
                _logger.LogWarning($"Registration attempt with null or empty password for email: {registerDto.Email}");
                throw new ArgumentException("Password is required");
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning($"Registration failed for {registerDto.Email}: {errors}");
                throw new ArgumentException($"Registration failed: {errors}");
            }

            // Assign default role
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new ApplicationRole("User"));
            }
            await _userManager.AddToRoleAsync(user, "User");

            // Send email verification OTP
            await SendOtpAsync(registerDto.Email, 1); // 1: Email Verification

            // Generate token for auto-login after email verification
            var userRoles = await _userManager.GetRolesAsync(user);
            var userPermissions = await GetUserPermissionsAsync(user.Id);

            var accessToken = _tokenService.GenerateAccessToken(user, userRoles, userPermissions);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var jwtId = new JwtSecurityToken(accessToken).Id;

            await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken, jwtId);

            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                User = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfileImageUrl = user.ProfileImageUrl ?? string.Empty,
                    Roles = userRoles.ToList(),
                    Permissions = userPermissions.ToList()
                }
            };
        }

        public async Task<TokenDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(refreshTokenDto.AccessToken);
            var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                _logger.LogWarning("Invalid access token during refresh token attempt");
                throw new ArgumentException("Invalid access token");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"Refresh token attempt for non-existent user: {userId}");
                throw new ArgumentException("User not found");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning($"Refresh token attempt by inactive user: {user.Email}");
                throw new ArgumentException("Account is disabled. Please contact support.");
            }

            var isValid = await _tokenService.ValidateRefreshTokenAsync(userId, refreshTokenDto.RefreshToken);
            if (!isValid)
            {
                _logger.LogWarning($"Invalid refresh token attempt for user: {user.Email}");
                throw new ArgumentException("Invalid refresh token");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var userPermissions = await GetUserPermissionsAsync(user.Id);

            var newAccessToken = _tokenService.GenerateAccessToken(user, userRoles, userPermissions);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var jwtId = new JwtSecurityToken(newAccessToken).Id;

            await _tokenService.SaveRefreshTokenAsync(user.Id, newRefreshToken, jwtId);

            return new TokenDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                // ... inside LoginAsync, RegisterAsync, and RefreshTokenAsync where UserDto is constructed:
                User = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    ProfileImageUrl = user.ProfileImageUrl ?? string.Empty,
                    Roles = userRoles.ToList(),
                    Permissions = userPermissions.ToList()
                }
            };
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"Logout attempt for non-existent user: {userId}");
                return false;
            }

            // Revoke all refresh tokens for this user
            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var refreshToken in refreshTokens)
            {
                refreshToken.IsRevoked = true;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"User {user.Email} logged out successfully");
            return true;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                _logger.LogInformation($"Password reset requested for non-existent email: {forgotPasswordDto.Email}");
                return true;
            }

            if (!user.IsActive)
            {
                _logger.LogWarning($"Password reset attempt for inactive user: {user.Email}");
                throw new ArgumentException("Account is disabled. Please contact support.");
            }

            // Send password reset OTP
            await SendOtpAsync(forgotPasswordDto.Email, 2); // 2: Password Reset

            _logger.LogInformation($"Password reset OTP sent to: {user.Email}");
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                _logger.LogWarning($"Password reset attempt for non-existent email: {resetPasswordDto.Email}");
                throw new ArgumentException("Invalid email or OTP");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning($"Password reset attempt for inactive user: {user.Email}");
                throw new ArgumentException("Account is disabled. Please contact support.");
            }

            // Validate OTP
            var isOtpValid = await _mfaService.ValidateOtpAsync(user.Id, resetPasswordDto.Otp, 2); // 2: Password Reset
            if (!isOtpValid)
            {
                _logger.LogWarning($"Invalid OTP provided for password reset by: {user.Email}");
                throw new ArgumentException("Invalid or expired OTP");
            }

            // Reset password
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning($"Password reset failed for {user.Email}: {errors}");
                throw new ArgumentException($"Password reset failed: {errors}");
            }

            // Mark OTP as used
            await _mfaService.MarkOtpAsUsedAsync(user.Id, resetPasswordDto.Otp);

            _logger.LogInformation($"Password reset successful for: {user.Email}");
            return true;
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            var user = await _userManager.FindByEmailAsync(verifyOtpDto.Email);
            if (user == null)
            {
                _logger.LogWarning($"OTP verification attempt for non-existent email: {verifyOtpDto.Email}");
                throw new ArgumentException("Invalid email or OTP");
            }

            var isOtpValid = await _mfaService.ValidateOtpAsync(user.Id, verifyOtpDto.Otp, verifyOtpDto.OtpType);
            if (!isOtpValid)
            {
                _logger.LogWarning($"Invalid OTP provided by: {user.Email}");
                throw new ArgumentException("Invalid or expired OTP");
            }

            // If it's email verification, mark email as confirmed
            if (verifyOtpDto.OtpType == 1) // 1: Email Verification
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning($"Email confirmation failed for {user.Email}: {errors}");
                    throw new ArgumentException($"Email confirmation failed: {errors}");
                }
            }

            // Mark OTP as used
            await _mfaService.MarkOtpAsUsedAsync(user.Id, verifyOtpDto.Otp);

            _logger.LogInformation($"OTP verification successful for: {user.Email}");
            return true;
        }

        public async Task<TokenDto> ExternalLoginAsync(ExternalAuthDto externalAuthDto)
        {
            // This would be implemented for Google, Facebook, etc.
            // For now, we'll throw a not implemented exception
            await Task.Yield();
            throw new NotImplementedException("External login not implemented yet");
        }

        public async Task<bool> SendOtpAsync(string email, int otpType)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning($"OTP send attempt for non-existent email: {email}");
                return false;
            }

            var otp = await _mfaService.GenerateOtpAsync(user.Id, otpType);
            await _emailService.SendOtpEmailAsync(email, otp, otpType);

            _logger.LogInformation($"OTP sent to: {email}");
            return true;
        }

        private async Task<IList<string>> GetUserPermissionsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new List<string>();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var permissions = new List<string>();

            foreach (var roleName in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    permissions.AddRange(roleClaims.Select(c => c.Value));
                }
            }

            return permissions;
        }
    }
}
