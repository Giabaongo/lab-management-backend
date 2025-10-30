using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabManagement.API.Controllers
{
    // COMMENTED OUT - Admin-specific controller (use UserController for admin operations instead)
    /*
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "4")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// Updates the role of a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="updateRoleDto">The request body containing the new role as a string.</param>
        [HttpPut("users/{userId}/role")]
        public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] UpdateUserDTO updateUserDTO)
        {
            // Step 1: Validate the incoming string and parse it into an enum
            if (updateUserDTO.Role == null)
            {
                return BadRequest(new { message = "Role is required." });
            }
            var roleValue = (int)updateUserDTO.Role.Value;
            if (updateUserDTO.Role.Value != Constant.UserRole.Admin)
            {
                // If the role is not Admin, return an error.
                return BadRequest(new { message = "Invalid role specified." });
            }
            else
            {
                roleValue = (int)Constant.UserRole.Admin;
            }
            // Step 2: Call the service with the validated data
            var success = await _adminService.UpdateUserRoleAsync(userId, (Constant.UserRole)roleValue);

            // Step 3: Return the correct response based on the service's result
            if (success)
            {
                return Ok(new { message = "User role updated successfully." });
            }
            else
            {
                return NotFound(new { message = "User not found." });
            }
        }
    }
    */
}
