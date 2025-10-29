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
        private readonly IGoogleAuthService _googleAuthService;

        public AuthController(IAuthService authService, IGoogleAuthService googleAuthService)
        {
            _authService = authService;
            _googleAuthService = googleAuthService;
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

        /// <summary>
        /// Login with Google account
        /// </summary>
        /// <param name="googleLoginDto">Google ID Token from frontend</param>
        /// <returns>JWT token and user info</returns>
        [HttpPost("google-login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> GoogleLogin([FromBody] GoogleLoginDTO googleLoginDto)
        {
            var result = await _googleAuthService.LoginWithGoogleAsync(googleLoginDto);
            
            return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result, "Google login successful"));
        }

        /// <summary>
        /// Test Google OAuth configuration
        /// </summary>
        [HttpGet("google-config")]
        public ActionResult<ApiResponse<object>> GetGoogleConfig()
        {
            var clientId = HttpContext.RequestServices.GetService<IConfiguration>()!["Google:ClientId"];
            
            return Ok(ApiResponse<object>.SuccessResponse(new 
            { 
                ClientId = clientId,
                IsConfigured = !string.IsNullOrEmpty(clientId)
            }, "Google configuration"));
        }
    }
}

