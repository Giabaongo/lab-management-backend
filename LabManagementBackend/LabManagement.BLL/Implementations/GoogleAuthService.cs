using Google.Apis.Auth;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Constants;
using LabManagement.Common.Exceptions;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LabManagement.BLL.Implementations;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GoogleAuthService> _logger;

    public GoogleAuthService(
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        ILogger<GoogleAuthService> logger)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AuthResponseDTO> LoginWithGoogleAsync(GoogleLoginDTO googleLogin)
    {
        if (string.IsNullOrEmpty(googleLogin.IdToken))
        {
            _logger.LogWarning("Google login attempted with null or empty token");
            throw new UnauthorizedException("Google ID token is required");
        }
        
        _logger.LogInformation("Attempting Google login with token: {TokenPrefix}...", 
            googleLogin.IdToken.Substring(0, Math.Min(20, googleLogin.IdToken.Length)));
        
        // Verify Google ID Token
        var payload = await VerifyGoogleTokenAsync(googleLogin.IdToken);
        
        if (payload == null)
        {
            _logger.LogWarning("Google token validation failed");
            throw new UnauthorizedException("Invalid Google token or configuration");
        }

        _logger.LogInformation("Google token validated successfully for email: {Email}", payload.Email);

        // Check if user exists
        var user = await _unitOfWork.Users.GetByEmailAsync(payload.Email);
        
        if (user == null)
        {
            // Create new user
            user = new User
            {
                Email = payload.Email,
                Name = payload.Name ?? payload.Email,
                PasswordHash = Guid.NewGuid().ToString(), // Random password for OAuth users
                Role = (int)Constant.UserRole.Member, // Default role
                CreatedAt = DateTime.UtcNow
            };
            
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        // Generate JWT token
        var token = GenerateJwtToken(user);
        
        return new AuthResponseDTO
        {
            Token = token,
            UserId = user.UserId,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role
        };
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim("UserId", user.UserId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, ((Constant.UserRole)user.Role).ToString()),
            new Claim("Role", user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string idToken)
    {
        try
        {
            var clientId = _configuration["Google:ClientId"];
            
            if (string.IsNullOrEmpty(clientId))
            {
                _logger.LogError("Google:ClientId is not configured in appsettings");
                return null;
            }
            
            _logger.LogInformation("Validating Google token with ClientId: {ClientId}", clientId);
            
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { clientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            
            _logger.LogInformation("Token validation successful. Email: {Email}, Name: {Name}", 
                payload.Email, payload.Name);
                
            return payload;
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning(ex, "Invalid JWT token");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Google token");
            return null;
        }
    }
}
