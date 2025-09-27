using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LabManagement.BLL.DTOs;
using LabManagement.DAL.Repos;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LabManagement.BLL.Services
{
    public class AuthService : IAuthService

    {
        private readonly IConfiguration _config;
        private readonly UserRepo _userRepo;

        public AuthService(IConfiguration config)
        {
            _config = config;
            _userRepo = new UserRepo();
        }
        public async Task<AuthResponseDTO> Login(LoginDTO loginDto)
        {
            var user = await _userRepo.Auth(loginDto.Email, loginDto.Password);
            if (user == null)
                return null!;

            // Create token
            var claims = new[]
            {
                new Claim("id", user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
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
