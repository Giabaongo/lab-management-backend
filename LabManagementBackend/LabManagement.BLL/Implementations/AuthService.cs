using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LabManagement.BLL.Implementations
{
    public class AuthService : IAuthService

    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(IConfiguration config, IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _config = config;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }
        public async Task<AuthResponseDTO> Login(LoginDTO loginDto)
        {
            // Get user by email
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            
            if (user == null)
                return null!;

            // Verify password using BCrypt
            if (!_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
                return null!;

            // Create token with enhanced claims
            var claims = new[]
            {
                new Claim("UserId", user.UserId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("Role", user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new AuthResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
    }
}
