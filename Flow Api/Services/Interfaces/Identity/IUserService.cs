using Flow_Api.Dtos.Common;
using Flow_Api.Dtos.User.Request;
using Flow_Api.Dtos.User.Response;
using Flow_Api.Dtos.Users.Request;
using Flow_Api.Models.Requests;

namespace Flow_Api.Services.Interfaces.Identity
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(string userId);
        Task<PaginatedResponse<UserDto>> GetUsersAsync(PaginationRequest pagination, SearchRequest search);
        Task<UserDto> UpdateUserAsync(UpdateUserRequestDto updateUserDto);
        Task<bool> DeleteUserAsync(string userId);
        Task<UserDto> AssignRolesAsync(AssignRoleDto assignRoleDto);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> UpdateProfileAsync(string userId, UpdateUserRequestDto updateUserDto);
    }
}
