using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace LabManagement.API.Controllers
{
    /// <summary>
    /// Authentication endpoints
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        /// <param name="loginDto">Login credentials</param>
        /// <returns>JWT token and user info</returns>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Login([FromBody] LoginDTO loginDto)
        {
            // Exceptions are thrown by AuthService and caught by ExceptionMiddleware
            var result = await _authService.Login(loginDto);
            
            return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result, "Login successful"));
        }
    }
}

