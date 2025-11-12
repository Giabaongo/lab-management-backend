using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace LabManagement.API.Controllers;

/// <summary>
/// Google OAuth callback endpoint
/// </summary>
[ApiController]
[Route("api/auth/google")]
public class GoogleOAuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _memoryCache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly ILogger<GoogleOAuthController> _logger;
    private const string StateCachePrefix = "google_oauth_state_";
    private static readonly TimeSpan StateTtl = TimeSpan.FromMinutes(5);

    public GoogleOAuthController(
        IConfiguration configuration,
        IMemoryCache memoryCache,
        IHttpClientFactory httpClientFactory,
        IGoogleAuthService googleAuthService,
        ILogger<GoogleOAuthController> logger)
    {
        _configuration = configuration;
        _memoryCache = memoryCache;
        _httpClientFactory = httpClientFactory;
        _googleAuthService = googleAuthService;
        _logger = logger;
    }

    /// <summary>
    /// Initiate Google OAuth flow
    /// </summary>
    [HttpGet("login")]
    public IActionResult Login([FromQuery] string? returnUrl = null)
    {
        var clientId = _configuration["Google:ClientId"];
        var redirectUri = BuildBackendRedirectUri();
        var scope = "openid profile email";
        var state = Guid.NewGuid().ToString("N");
        var frontendCallback = string.IsNullOrWhiteSpace(returnUrl)
            ? GetDefaultFrontendCallbackUrl()
            : returnUrl!;

        if (!Uri.TryCreate(frontendCallback, UriKind.Absolute, out var validatedCallback))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("returnUrl must be an absolute URL"));
        }
        frontendCallback = validatedCallback.ToString();

        if (string.IsNullOrWhiteSpace(clientId))
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResponse("Google OAuth client id is not configured"));
        }

        _memoryCache.Set(GetStateCacheKey(state), new GoogleOAuthState(frontendCallback),
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = StateTtl
            });

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
    public async Task<IActionResult> Callback(
        [FromQuery] string code,
        [FromQuery] string state,
        [FromQuery(Name = "error")] string? googleError = null)
    {
        if (!string.IsNullOrEmpty(googleError))
        {
            _logger.LogWarning("Google returned an error during OAuth callback: {Error}", googleError);
            return RedirectWithError(GetDefaultFrontendCallbackUrl(), googleError);
        }

        if (string.IsNullOrEmpty(code))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Authorization code is missing"));
        }

        if (string.IsNullOrEmpty(state))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("OAuth state is missing"));
        }

        if (!_memoryCache.TryGetValue(GetStateCacheKey(state), out GoogleOAuthState? oauthState))
        {
            _logger.LogWarning("Invalid or expired OAuth state: {State}", state);
            return BadRequest(ApiResponse<object>.ErrorResponse("Invalid or expired OAuth state"));
        }

        _memoryCache.Remove(GetStateCacheKey(state));

        var tokenResponse = await ExchangeCodeForTokensAsync(code, BuildBackendRedirectUri());
        if (tokenResponse?.IdToken is null)
        {
            return RedirectWithError(oauthState.CallbackUrl, "Failed to exchange authorization code");
        }

        var authResult = await _googleAuthService.LoginWithGoogleAsync(new GoogleLoginDTO
        {
            IdToken = tokenResponse.IdToken
        });

        var redirectUrl = QueryHelpers.AddQueryString(oauthState.CallbackUrl, new Dictionary<string, string?>
        {
            ["token"] = authResult.Token,
            ["userId"] = authResult.UserId.ToString(),
            ["email"] = authResult.Email,
            ["name"] = authResult.Name,
            ["role"] = authResult.Role.ToString(),
            ["state"] = state
        });

        return Redirect(redirectUrl);
    }

    private string BuildBackendRedirectUri()
    {
        return $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/auth/google/callback";
    }

    private string GetDefaultFrontendCallbackUrl()
    {
        var configuredCallback = _configuration["FrontendCallbackUrl"];
        if (!string.IsNullOrWhiteSpace(configuredCallback))
        {
            return configuredCallback!;
        }

        var frontendBase = _configuration["FrontendUrl"] ?? "http://localhost:3000";
        return $"{frontendBase.TrimEnd('/')}/auth/callback";
    }

    private string GetStateCacheKey(string state) => $"{StateCachePrefix}{state}";

    private async Task<GoogleTokenResponse?> ExchangeCodeForTokensAsync(string code, string redirectUri)
    {
        var clientId = _configuration["Google:ClientId"];
        var clientSecret = _configuration["Google:ClientSecret"];

        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
        {
            _logger.LogError("Google OAuth client configuration is missing (ClientId or ClientSecret)");
            return null;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            var formValues = new Dictionary<string, string?>
            {
                ["code"] = code,
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["redirect_uri"] = redirectUri,
                ["grant_type"] = "authorization_code"
            };

            var response = await client.PostAsync("https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(formValues));

            if (!response.IsSuccessStatusCode)
            {
                var errorPayload = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to exchange code for tokens. Status: {Status}, Body: {Body}",
                    response.StatusCode, errorPayload);
                return null;
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();
            return tokenResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while exchanging Google authorization code for tokens");
            return null;
        }
    }

    private IActionResult RedirectWithError(string callbackUrl, string error)
    {
        var target = QueryHelpers.AddQueryString(callbackUrl, new Dictionary<string, string?>
        {
            ["error"] = error
        });

        return Redirect(target);
    }

    private sealed record GoogleOAuthState(string CallbackUrl);

    private sealed class GoogleTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("id_token")]
        public string? IdToken { get; set; }
    }
}
