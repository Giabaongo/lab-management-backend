using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Constants;
using LabManagement.Common.Exceptions;
using LabManagement.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LabManagement.API.Controllers
{
    /// <summary>
    /// User management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users (Admin and SchoolManager only)
        /// </summary>
        /// <returns>List of users</returns>
        [HttpGet]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDTO>>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(ApiResponse<IEnumerable<UserDTO>>.SuccessResponse(users, "Users retrieved successfully"));
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.LabManager)},{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult<ApiResponse<UserDTO>>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            
            if (user == null)
                throw new NotFoundException("User", id);

            return Ok(ApiResponse<UserDTO>.SuccessResponse(user, "User retrieved successfully"));
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns>User details</returns>
        [HttpGet("email/{email}")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.LabManager)},{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult<ApiResponse<UserDTO>>> GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BadRequestException("Email cannot be empty");

            var user = await _userService.GetUserByEmailAsync(email);
            
            if (user == null)
                throw new NotFoundException($"User with email '{email}' was not found");

            return Ok(ApiResponse<UserDTO>.SuccessResponse(user, "User retrieved successfully"));
        }

        /// <summary>
        /// Create a new user (Admin only)
        /// </summary>
        /// <param name="createUserDto">User creation data</param>
        /// <returns>Created user</returns>
        [AllowAnonymous]
        [HttpPost]
        [Authorize(Roles = nameof(Constant.UserRole.Admin))]
        public async Task<ActionResult<ApiResponse<UserDTO>>> CreateUser([FromBody] CreateUserDTO createUserDto)
        {
            if (!ModelState.IsValid)
                throw new BadRequestException("Invalid user data");

            // Check if email already exists
            if (await _userService.EmailExistsAsync(createUserDto.Email))
                throw new BadRequestException($"Email '{createUserDto.Email}' already exists");

            var user = await _userService.CreateUserAsync(createUserDto);
            
            return CreatedAtAction(
                nameof(GetUserById), 
                new { id = user.UserId }, 
                ApiResponse<UserDTO>.SuccessResponse(user, "User created successfully")
            );
        }

        /// <summary>
        /// Update user by ID (Admin and SchoolManager only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="updateUserDto">User update data</param>
        /// <returns>Updated user</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult<ApiResponse<UserDTO>>> UpdateUser(int id, [FromBody] UpdateUserDTO updateUserDto)
        {
            if (!ModelState.IsValid)
                throw new BadRequestException("Invalid user data");

            // Check if user exists
            if (!await _userService.UserExistsAsync(id))
                throw new NotFoundException("User", id);

            // Check if email already exists (if updating email)
            if (!string.IsNullOrEmpty(updateUserDto.Email))
            {
                var existingUser = await _userService.GetUserByEmailAsync(updateUserDto.Email);
                if (existingUser != null && existingUser.UserId != id)
                    throw new BadRequestException($"Email '{updateUserDto.Email}' already exists");
            }

            var user = await _userService.UpdateUserAsync(id, updateUserDto);
            
            if (user == null)
                throw new NotFoundException("User", id);

            return Ok(ApiResponse<UserDTO>.SuccessResponse(user, "User updated successfully"));
        }

        /// <summary>
        /// Delete user by ID (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(Constant.UserRole.Admin))]
        public async Task<ActionResult<ApiResponse<object>>> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            
            if (!result)
                throw new NotFoundException("User", id);

            return Ok(ApiResponse<object>.SuccessResponse(
                new { deletedUserId = id }, 
                "User deleted successfully"
            ));
        }

        /// <summary>
        /// Check if user exists
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Boolean result</returns>
        [HttpGet("{id}/exists")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.LabManager)},{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult<ApiResponse<object>>> UserExists(int id)
        {
            var exists = await _userService.UserExistsAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(
                new { userId = id, exists }, 
                exists ? "User exists" : "User does not exist"
            ));
        }

        /// <summary>
        /// Check if email exists
        /// </summary>
        /// <param name="email">Email address</param>
        /// <returns>Boolean result</returns>
        [HttpGet("email/{email}/exists")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.LabManager)},{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult<ApiResponse<object>>> EmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BadRequestException("Email cannot be empty");

            var exists = await _userService.EmailExistsAsync(email);
            return Ok(ApiResponse<object>.SuccessResponse(
                new { email, exists }, 
                exists ? "Email exists" : "Email does not exist"
            ));
        }
    }
}


