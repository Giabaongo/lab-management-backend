using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Exceptions;
using LabManagement.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabManagement.API.Controllers
{
    /// <summary>
    /// Example controller showing how to use ApiResponse and Custom Exceptions
    /// </summary>
    [ApiController]
    [Route("api/examples")]
    [Authorize]
    public class ExampleController : ControllerBase
    {
        private readonly IUserService _userService;

        public ExampleController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Example 1: Return success response with ApiResponse wrapper
        /// </summary>
        [HttpGet("success")]
        [AllowAnonymous]
        public ActionResult<ApiResponse<string>> GetSuccessExample()
        {
            return Ok(ApiResponse<string>.SuccessResponse("This is a success message", "Operation completed successfully"));
        }

        /// <summary>
        /// Example 2: Throw NotFoundException (will be caught by ExceptionMiddleware)
        /// </summary>
        [HttpGet("not-found/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<UserDTO>>> GetNotFoundExample(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            
            if (user == null)
            {
                // This will be caught by ExceptionMiddleware and return 404
                throw new NotFoundException("User", id);
            }

            return Ok(ApiResponse<UserDTO>.SuccessResponse(user, "User retrieved successfully"));
        }

        /// <summary>
        /// Example 3: Throw BadRequestException
        /// </summary>
        [HttpPost("bad-request")]
        [AllowAnonymous]
        public ActionResult<ApiResponse<object>> PostBadRequestExample([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                // This will be caught by ExceptionMiddleware and return 400
                throw new BadRequestException("Invalid email format");
            }

            return Ok(ApiResponse<string>.SuccessResponse("Valid email", "Email validation passed"));
        }

        /// <summary>
        /// Example 4: Throw UnauthorizedException
        /// </summary>
        [HttpGet("unauthorized")]
        [AllowAnonymous]
        public ActionResult<ApiResponse<object>> GetUnauthorizedExample()
        {
            // Simulate unauthorized access
            var isAuthorized = false;
            
            if (!isAuthorized)
            {
                // This will be caught by ExceptionMiddleware and return 401
                throw new UnauthorizedException("You are not authorized to access this resource");
            }

            return Ok(ApiResponse<string>.SuccessResponse("Secret data", "Access granted"));
        }

        /// <summary>
        /// Example 5: Throw unhandled exception (will return 500)
        /// </summary>
        [HttpGet("error")]
        [AllowAnonymous]
        public ActionResult<ApiResponse<object>> GetErrorExample()
        {
            // This will be caught by ExceptionMiddleware and return 500
            // In Development: will show full error details
            // In Production: will show generic error message
            throw new Exception("Something went terribly wrong!");
        }

        /// <summary>
        /// Example 6: Manual error response (without throwing exception)
        /// </summary>
        [HttpGet("manual-error")]
        [AllowAnonymous]
        public ActionResult<ApiResponse<object>> GetManualErrorExample()
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                "Manual validation error",
                new List<string> { "Field1 is required", "Field2 must be greater than 0" }
            ));
        }
    }
}
