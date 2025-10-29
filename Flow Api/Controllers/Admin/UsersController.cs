using Flow_Api.Dtos.Common;
using Flow_Api.Dtos.User;
using Flow_Api.Dtos.User.Request;
using Flow_Api.Dtos.Users.Request;
using Flow_Api.Models.Requests;
using Flow_Api.Services.Interfaces;
using Flow_Api.Services.Interfaces.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flow_Api.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(ApiResponse<object>.ErrorResponse("User ID is required"));

            try
            {
                var user = await _userService.GetUserByIdAsync(userId);

                if (user == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

                return Ok(ApiResponse<object>.SuccessResponse(user, "User retrieved successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse($"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] PaginationRequest? paginationRequest,
            [FromQuery] SearchRequest? searchRequest)
        {
            try
            {
                var pagination = paginationRequest ?? new PaginationRequest();
                var search = searchRequest ?? new SearchRequest();

                var users = await _userService.GetUsersAsync(pagination, search);
                return Ok(ApiResponse<object>.SuccessResponse(users, "Users retrieved successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse($"Internal server error: {ex.Message}"));
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserRequestDto? updateUserRequestDto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(ApiResponse<object>.ErrorResponse("User ID is required"));

            if (updateUserRequestDto == null)
                return BadRequest(ApiResponse<object>.ErrorResponse("User data is required"));

            try
            {
                // Ensure the ID in the URL matches the ID in the request body
                if (userId != updateUserRequestDto.Id)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch"));
                }

                var user = await _userService.UpdateUserAsync(updateUserRequestDto);
                return Ok(ApiResponse<object>.SuccessResponse(user, "User updated successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse($"Internal server error: {ex.Message}"));
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(ApiResponse<object>.ErrorResponse("User ID is required"));

            try
            {
                var result = await _userService.DeleteUserAsync(userId);

                if (result)
                {
                    return Ok(ApiResponse<object>.SuccessResponse(null, "User deleted successfully"));
                }

                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to delete user"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse($"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("assign-roles")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> AssignRoles([FromBody] AssignRoleDto? assignRoleDto)
        {
            if (assignRoleDto == null)
                return BadRequest(ApiResponse<object>.ErrorResponse("Role assignment data is required"));

            try
            {
                var user = await _userService.AssignRolesAsync(assignRoleDto);
                return Ok(ApiResponse<object>.SuccessResponse(user, "Role assigned successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse($"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto? changePasswordDto)
        {
            if (changePasswordDto == null)
                return BadRequest(ApiResponse<object>.ErrorResponse("Password change data is required"));

            try
            {
                var result = await _userService.ChangePasswordAsync(
                    changePasswordDto.UserId,
                    changePasswordDto.CurrentPassword,
                    changePasswordDto.NewPassword);

                if (result)
                {
                    return Ok(ApiResponse<object>.SuccessResponse(null, "Password changed successfully"));
                }

                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to change password"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse($"Internal server error: {ex.Message}"));
            }
        }

        [HttpPut("profile/{userId}")]
        public async Task<IActionResult> UpdateProfile(string userId, [FromBody] UpdateUserRequestDto? updateUserDto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(ApiResponse<object>.ErrorResponse("User ID is required"));

            if (updateUserDto == null)
                return BadRequest(ApiResponse<object>.ErrorResponse("Profile data is required"));

            try
            {
                // Ensure the ID in the URL matches the ID in the request body
                if (userId != updateUserDto.Id)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch"));
                }

                var result = await _userService.UpdateProfileAsync(userId, updateUserDto);

                if (result)
                {
                    return Ok(ApiResponse<object>.SuccessResponse(null, "Profile updated successfully"));
                }

                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to update profile"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse($"Internal server error: {ex.Message}"));
            }
        }
    }
}
