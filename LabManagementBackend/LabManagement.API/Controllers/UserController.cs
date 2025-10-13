using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of users</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns>User details</returns>
        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserDTO>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="createUserDto">User creation data</param>
        /// <returns>Created user</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] CreateUserDTO createUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if email already exists
                if (await _userService.EmailExistsAsync(createUserDto.Email))
                {
                    return Conflict(new { message = "Email already exists" });
                }

                var user = await _userService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Update user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="updateUserDto">User update data</param>
        /// <returns>Updated user</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDTO>> UpdateUser(int id, [FromBody] UpdateUserDTO updateUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if user exists
                if (!await _userService.UserExistsAsync(id))
                {
                    return NotFound(new { message = "User not found" });
                }

                // Check if email already exists (if updating email)
                if (!string.IsNullOrEmpty(updateUserDto.Email))
                {
                    var existingUser = await _userService.GetUserByEmailAsync(updateUserDto.Email);
                    if (existingUser != null && existingUser.UserId != id)
                    {
                        return Conflict(new { message = "Email already exists" });
                    }
                }

                var user = await _userService.UpdateUserAsync(id, updateUserDto);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if user exists
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Boolean result</returns>
        [HttpGet("{id}/exists")]
        public async Task<ActionResult<bool>> UserExists(int id)
        {
            try
            {
                var exists = await _userService.UserExistsAsync(id);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if email exists
        /// </summary>
        /// <param name="email">Email address</param>
        /// <returns>Boolean result</returns>
        [HttpGet("email/{email}/exists")]
        public async Task<ActionResult<bool>> EmailExists(string email)
        {
            try
            {
                var exists = await _userService.EmailExistsAsync(email);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}