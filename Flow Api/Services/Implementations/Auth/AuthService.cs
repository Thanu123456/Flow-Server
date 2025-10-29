using Flow_Api.Data.UnitOfWork;
using Flow_Api.Dtos.Auth.Request;
using Flow_Api.Dtos.Auth.Response;
using Flow_Api.Exceptions;
using Flow_Api.Models.Entities.Enums;
using Flow_Api.Models.Entities.Master;
using Flow_Api.Services.Interfaces;
using Flow_Api.Services.Interfaces.Auth;
using Flow_Api.Services.Interfaces.Notifications;
using Flow_Api.Services.Interfaces.SuperAdmin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TenantEntity = Flow_Api.Models.Entities.Master.Tenant;

namespace Flow_Api.Services.Implementations.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IMasterUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IAuditLogService _auditLogService;

        public AuthService(
            IMasterUnitOfWork unitOfWork,
            IPasswordService passwordService,
            ITokenService tokenService,
            IEmailService emailService,
            IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _emailService = emailService;
            _auditLogService = auditLogService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string ipAddress)
        {
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                throw new UnauthorizedException("Invalid email or password");

            // Check if account is locked
            if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
                throw new UnauthorizedException($"Account is locked until {user.LockedUntil.Value:yyyy-MM-dd HH:mm}");

            // Verify password
            if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockedUntil = DateTime.UtcNow.AddMinutes(15);
                    await _unitOfWork.SaveChangesAsync();
                    throw new UnauthorizedException("Too many failed attempts. Account locked for 15 minutes");
                }

                await _unitOfWork.SaveChangesAsync();
                throw new UnauthorizedException("Invalid email or password");
            }

            // Check account status
            if (user.Status != UserStatus.Active)
                throw new UnauthorizedException("Account is not active");

            // Check if super admin or tenant status
            if (!user.IsSuperAdmin && user.TenantId.HasValue)
            {
                var userTenant = await _unitOfWork.Tenants.GetByIdAsync(user.TenantId.Value);
                if (userTenant == null || userTenant.RegistrationStatus != RegistrationStatus.Active)
                    throw new UnauthorizedException("Your account is pending approval or has been suspended");
            }

            // Reset failed attempts
            user.FailedLoginAttempts = 0;
            user.LockedUntil = null;
            user.LastLoginAt = DateTime.UtcNow;
            user.LastLoginIp = ipAddress;

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

            await _unitOfWork.SaveChangesAsync();

            // Log login
            await _auditLogService.LogActionAsync(
                user.IsSuperAdmin ? AuditActionType.SuperAdminLogin : AuditActionType.UserCreated,
                $"{user.FullName} logged in",
                user.Id,
                user.TenantId,
                ipAddress
            );

            // Get tenant info if exists
            TenantEntity? loginTenant = null;
            if (user.TenantId.HasValue)
            {
                loginTenant = await _unitOfWork.Tenants.GetByIdAsync(user.TenantId.Value);
            }

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                User = new UserInfoResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    IsSuperAdmin = user.IsSuperAdmin,
                    TenantId = user.TenantId,
                    TenantName = loginTenant?.ShopName,
                    SchemaName = loginTenant?.SchemaName,
                    ProfileImageUrl = user.ProfileImageUrl
                }
            };
        }

        public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request, string ipAddress)
        {
            // Check if email already exists
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
                throw new BadRequestException("Email already registered");

            // Validate password strength
            if (!_passwordService.IsPasswordStrong(request.Password))
                throw new BadRequestException("Password does not meet security requirements");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Create owner user
                var owner = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    PasswordHash = _passwordService.HashPassword(request.Password),
                    Status = UserStatus.PendingApproval,
                    IsSuperAdmin = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(owner);
                await _unitOfWork.SaveChangesAsync();

                // Create tenant
                var newTenant = new TenantEntity
                {
                    ShopName = request.ShopName,
                    BusinessType = request.BusinessType,
                    BusinessRegistrationNumber = request.BusinessRegistrationNumber,
                    TaxNumber = request.TaxNumber,
                    AddressLine1 = request.AddressLine1,
                    AddressLine2 = request.AddressLine2,
                    City = request.City,
                    PostalCode = request.PostalCode,
                    OwnerId = owner.Id,
                    RegistrationStatus = RegistrationStatus.Pending,
                    IpAddress = ipAddress,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Tenants.AddAsync(newTenant);

                // Link tenant to owner
                owner.TenantId = newTenant.Id;

                await _unitOfWork.SaveChangesAsync();

                // Log registration
                await _auditLogService.LogActionAsync(
                    AuditActionType.RegistrationSubmitted,
                    $"New registration: {request.ShopName}",
                    owner.Id,
                    newTenant.Id,
                    ipAddress
                );

                await _unitOfWork.CommitTransactionAsync();

                // Send welcome email
                await _emailService.SendWelcomeEmailAsync(owner.Email, owner.FullName, newTenant.ShopName);

                // Generate response (no login until approved)
                return new LoginResponseDto
                {
                    AccessToken = string.Empty,
                    RefreshToken = string.Empty,
                    ExpiresAt = DateTime.UtcNow,
                    User = new UserInfoResponseDto
                    {
                        Id = owner.Id,
                        FullName = owner.FullName,
                        Email = owner.Email,
                        IsSuperAdmin = false,
                        TenantId = newTenant.Id,
                        TenantName = newTenant.ShopName
                    }
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task LogoutAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiresAt = null;
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedException("Invalid or expired refresh token");

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

            await _unitOfWork.SaveChangesAsync();

            TenantEntity? refreshTenant = null;
            if (user.TenantId.HasValue)
            {
                refreshTenant = await _unitOfWork.Tenants.GetByIdAsync(user.TenantId.Value);
            }

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                User = new UserInfoResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    IsSuperAdmin = user.IsSuperAdmin,
                    TenantId = user.TenantId,
                    TenantName = refreshTenant?.ShopName,
                    SchemaName = refreshTenant?.SchemaName
                }
            };
        }

        public Task<bool> ForgotPasswordAsync(string email)
        {
            // Implementation for password reset
            throw new NotImplementedException();
        }

        public Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            throw new NotImplementedException();
        }
    }
}
