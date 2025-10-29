using AutoMapper;
using Flow_Api.Data.UnitOfWork;
using Flow_Api.Dtos.Common;
using Flow_Api.Dtos.User.Request;
using Flow_Api.Dtos.User.Response;
using Flow_Api.Dtos.Users.Request;
using Flow_Api.Exceptions;
using Flow_Api.Models.Requests;
using Flow_Api.Services.Interfaces.Auth;
using Flow_Api.Services.Interfaces.Identity;

namespace Flow_Api.Services.Implementations.Identity
{
    public class UserService : IUserService
    {
        private readonly IMasterUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;

        public UserService(
            IMasterUnitOfWork unitOfWork,
            IMapper mapper,
            IPasswordService passwordService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
        }

        public async Task<UserDto?> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new BadRequestException("User ID is required");

            if (!Guid.TryParse(userId, out var userGuid))
                throw new BadRequestException("Invalid user ID format");

            var user = await _unitOfWork.Users.GetByIdAsync(userGuid);

            if (user == null)
                return null;

            return _mapper.Map<UserDto>(user);
        }

        public async Task<PaginatedResponse<UserDto>> GetUsersAsync(
            PaginationRequest pagination,
            SearchRequest search)
        {
            if (pagination == null)
                throw new ArgumentNullException(nameof(pagination));

            if (search == null)
                search = new SearchRequest();

            var allUsers = await _unitOfWork.Users.GetAllAsync();
            var query = allUsers.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search.SearchTerm))
            {
                var searchLower = search.SearchTerm.ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(searchLower) ||
                    u.Email.ToLower().Contains(searchLower) ||
                    u.PhoneNumber.Contains(searchLower));
            }

            // Apply role filter
            if (search.RoleId.HasValue)
            {
                // TODO: Filter by role when role system is implemented
                // query = query.Where(u => u.RoleId == search.RoleId.Value);
            }

            // Apply active status filter
            if (search.IsActive.HasValue)
            {
                query = query.Where(u => !u.IsDeleted == search.IsActive.Value);
            }

            // Apply sorting
            var sortBy = search.SortBy?.ToLower() ?? "createdat";
            var sortOrder = search.SortOrder?.ToLower() ?? "desc";

            query = sortBy switch
            {
                "name" => sortOrder == "desc"
                    ? query.OrderByDescending(u => u.FullName)
                    : query.OrderBy(u => u.FullName),
                "email" => sortOrder == "desc"
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email),
                "createdat" => sortOrder == "desc"
                    ? query.OrderByDescending(u => u.CreatedAt)
                    : query.OrderBy(u => u.CreatedAt),
                _ => query.OrderByDescending(u => u.CreatedAt)
            };

            // Get total count
            var totalCount = query.Count();

            // Apply pagination
            var users = query
                .Skip(pagination.Skip)
                .Take(pagination.Take)
                .ToList();

            var userDtos = _mapper.Map<List<UserDto>>(users);

            return new PaginatedResponse<UserDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<UserDto> UpdateUserAsync(UpdateUserRequestDto updateUserDto)
        {
            if (updateUserDto == null)
                throw new ArgumentNullException(nameof(updateUserDto));

            if (string.IsNullOrWhiteSpace(updateUserDto.Id))
                throw new BadRequestException("User ID is required");

            if (!Guid.TryParse(updateUserDto.Id, out var userGuid))
                throw new BadRequestException("Invalid user ID format");

            var user = await _unitOfWork.Users.GetByIdAsync(userGuid);

            if (user == null)
                throw new NotFoundException("User not found");

            // Update user properties
            user.FullName = updateUserDto.FullName ?? user.FullName;

            if (!string.IsNullOrWhiteSpace(updateUserDto.Email) && updateUserDto.Email != user.Email)
            {
                // Check if email is already taken
                var emailExists = await _unitOfWork.Users.ExistsAsync(u =>
                    u.Email == updateUserDto.Email && u.Id != userGuid);

                if (emailExists)
                    throw new BadRequestException("Email is already in use");

                user.Email = updateUserDto.Email;
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.PhoneNumber))
                user.PhoneNumber = updateUserDto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(updateUserDto.ProfileImageUrl))
                user.ProfileImageUrl = updateUserDto.ProfileImageUrl;

            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new BadRequestException("User ID is required");

            if (!Guid.TryParse(userId, out var userGuid))
                throw new BadRequestException("Invalid user ID format");

            var user = await _unitOfWork.Users.GetByIdAsync(userGuid);

            if (user == null)
                throw new NotFoundException("User not found");

            // Prevent deleting super admin
            if (user.IsSuperAdmin)
                throw new BadRequestException("Cannot delete super admin user");

            // Soft delete
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<UserDto> AssignRolesAsync(AssignRoleDto assignRoleDto)
        {
            if (assignRoleDto == null)
                throw new ArgumentNullException(nameof(assignRoleDto));

            if (string.IsNullOrWhiteSpace(assignRoleDto.UserId))
                throw new BadRequestException("User ID is required");

            if (!Guid.TryParse(assignRoleDto.UserId, out var userGuid))
                throw new BadRequestException("Invalid user ID format");

            var user = await _unitOfWork.Users.GetByIdAsync(userGuid);

            if (user == null)
                throw new NotFoundException("User not found");

            // TODO: Implement role assignment when tenant role system is ready
            // For now, we'll just update the user record
            // This will need to interact with the tenant-specific role tables

            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> ChangePasswordAsync(
            string userId,
            string currentPassword,
            string newPassword)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new BadRequestException("User ID is required");

            if (string.IsNullOrWhiteSpace(currentPassword))
                throw new BadRequestException("Current password is required");

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new BadRequestException("New password is required");

            if (!Guid.TryParse(userId, out var userGuid))
                throw new BadRequestException("Invalid user ID format");

            var user = await _unitOfWork.Users.GetByIdAsync(userGuid);

            if (user == null)
                throw new NotFoundException("User not found");

            // Verify current password
            if (!_passwordService.VerifyPassword(currentPassword, user.PasswordHash))
                throw new BadRequestException("Current password is incorrect");

            // Validate new password strength
            if (!_passwordService.IsPasswordStrong(newPassword))
                throw new BadRequestException("New password does not meet security requirements");

            // Hash new password
            user.PasswordHash = _passwordService.HashPassword(newPassword);
            user.LastPasswordChangedAt = DateTime.UtcNow;
            user.MustChangePassword = false;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateProfileAsync(string userId, UpdateUserRequestDto updateUserDto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new BadRequestException("User ID is required");

            if (updateUserDto == null)
                throw new ArgumentNullException(nameof(updateUserDto));

            if (!Guid.TryParse(userId, out var userGuid))
                throw new BadRequestException("Invalid user ID format");

            var user = await _unitOfWork.Users.GetByIdAsync(userGuid);

            if (user == null)
                throw new NotFoundException("User not found");

            // Update profile fields
            if (!string.IsNullOrWhiteSpace(updateUserDto.FullName))
                user.FullName = updateUserDto.FullName;

            if (!string.IsNullOrWhiteSpace(updateUserDto.PhoneNumber))
                user.PhoneNumber = updateUserDto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(updateUserDto.ProfileImageUrl))
                user.ProfileImageUrl = updateUserDto.ProfileImageUrl;

            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
