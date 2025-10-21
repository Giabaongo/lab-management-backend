using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Exceptions;
using LabManagement.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LabManagement.BLL.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(IConfiguration config, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResponseDTO> Login(LoginDTO loginDto)
        {
            // Validate input
            if (string.IsNullOrEmpty(loginDto.Email))
                throw new BadRequestException("Email is required");
            
            if (string.IsNullOrEmpty(loginDto.Password))
                throw new BadRequestException("Password is required");

            // Get user by email
            var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);
            
            if (user == null)
                throw new UnauthorizedException("Invalid email or password");

            // Verify password using BCrypt
            if (!_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
                throw new UnauthorizedException("Invalid email or password");

            // Create token with enhanced claims
            var claims = new[]
            {
                new Claim("UserId", user.UserId.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, ((Common.Constants.Constant.UserRole)user.Role).ToString()),
                new Claim("Role", user.Role.ToString(CultureInfo.InvariantCulture))
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
