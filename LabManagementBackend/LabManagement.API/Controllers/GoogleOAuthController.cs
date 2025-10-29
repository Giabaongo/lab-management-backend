using LabManagement.BLL.DTOs;
using LabManagement.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LabManagement.API.Controllers;

/// <summary>
/// Google OAuth callback endpoint
/// </summary>
[ApiController]
[Route("api/auth/google")]
public class GoogleOAuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public GoogleOAuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Initiate Google OAuth flow
    /// </summary>
    [HttpGet("login")]
    public IActionResult Login()
    {
        var clientId = _configuration["Google:ClientId"];
        var redirectUri = $"{Request.Scheme}://{Request.Host}/api/auth/google/callback";
        var scope = "openid profile email";
        var state = Guid.NewGuid().ToString(); // In production, store this in session/cache

        var authUrl = "https://accounts.google.com/o/oauth2/v2/auth" +
            $"?client_id={Uri.EscapeDataString(clientId!)}" +
            $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
            $"&response_type=code" +
            $"&scope={Uri.EscapeDataString(scope)}" +
            $"&state={state}" +
            $"&access_type=offline" +
            $"&prompt=consent";

        return Redirect(authUrl);
    }

    /// <summary>
    /// Google OAuth callback
    /// </summary>
    [HttpGet("callback")]
    public IActionResult Callback([FromQuery] string code, [FromQuery] string state)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Authorization code is missing"));
        }

        // TODO: Exchange code for tokens and validate
        // For now, redirect to frontend with code
        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:3000";
        return Redirect($"{frontendUrl}/auth/callback?code={code}&state={state}");
    }
}
