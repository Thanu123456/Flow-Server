using Flow_Api.Dtos.User;
using Flow_Api.Requests;
using Flow_Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flow_Api.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationRequest paginationRequest, [FromQuery] SearchRequest searchRequest)
        {
            try
            {
                var users = await _userService.GetUsersAsync(paginationRequest, searchRequest);
                return Ok(users);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                // Ensure the ID in the URL matches the ID in the request body
                if (userId != updateUserDto.Id)
                {
                    return BadRequest(new { message = "ID mismatch" });
                }

                var user = await _userService.UpdateUserAsync(updateUserDto);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(userId);
                if (result)
                {
                    return Ok(new { message = "User deleted successfully" });
                }
                return BadRequest(new { message = "Failed to delete user" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("assign-roles")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> AssignRoles([FromBody] AssignRoleDto assignRoleDto)
        {
            try
            {
                var user = await _userService.AssignRolesAsync(assignRoleDto);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var result = await _userService.ChangePasswordAsync(
                    changePasswordDto.UserId,
                    changePasswordDto.CurrentPassword,
                    changePasswordDto.NewPassword);

                if (result)
                {
                    return Ok(new { message = "Password changed successfully" });
                }
                return BadRequest(new { message = "Failed to change password" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("profile/{userId}")]
        public async Task<IActionResult> UpdateProfile(string userId, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                // Ensure the ID in the URL matches the ID in the request body
                if (userId != updateUserDto.Id)
                {
                    return BadRequest(new { message = "ID mismatch" });
                }

                var result = await _userService.UpdateProfileAsync(userId, updateUserDto);
                if (result)
                {
                    return Ok(new { message = "Profile updated successfully" });
                }
                return BadRequest(new { message = "Failed to update profile" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public class ChangePasswordDto
        {
            public string UserId { get; set; }
            public string CurrentPassword { get; set; }
            public string NewPassword { get; set; }
        }
    }
}
