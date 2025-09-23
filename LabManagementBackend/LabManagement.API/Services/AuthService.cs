using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LabManagement.API.DTOs;
using LabManagement.API.Repos;
using Microsoft.IdentityModel.Tokens;

namespace LabManagement.API.Services
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
            var user = _userRepo.Auth(loginDto.Username, loginDto.Password);
            if (user == null)
                return null;

            // Create token
            var claims = new[]
            {
                new Claim("id", "123"),
                new Claim(ClaimTypes.Name, "testuser")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
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
