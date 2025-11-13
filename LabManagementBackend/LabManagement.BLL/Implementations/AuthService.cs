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
using StackExchange.Redis; // 1. Add this
using System.Net;

namespace LabManagement.BLL.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        // 2. Add Redis-related fields
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _redisDb;
        private const int MaxLoginAttempts = 5;
        private const int LockoutMinutes = 10;

        public AuthService(IConfiguration config, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;

            // 3. Initialize the Redis connection manually
            try
            {
                var redisConnectionString = _config["Redis:ConnectionString"];
                if (!string.IsNullOrEmpty(redisConnectionString))
                {
                    _redis = ConnectionMultiplexer.Connect(redisConnectionString);
                    _redisDb = _redis.GetDatabase();
                }
            }
            catch (RedisConnectionException ex)
            {
                // Log this, but don't crash the app
                Console.WriteLine($"Failed to connect to Redis: {ex.Message}");
            }
        }

        public async Task<AuthResponseDTO> Login(LoginDTO loginDto)
        {
            // Validate input
            if (string.IsNullOrEmpty(loginDto.Email))
                throw new BadRequestException("Email is required");
            
            if (string.IsNullOrEmpty(loginDto.Password))
                throw new BadRequestException("Password is required");

            // --- 4. REDIS: Check for lockout ---
            string loginAttemptKey = $"login_attempts:{loginDto.Email}";
            if (_redisDb != null)
            {
                try
                {
                    var attempts = (int)await _redisDb.StringGetAsync(loginAttemptKey);
                    if (attempts >= MaxLoginAttempts)
                    {
                        throw new UnauthorizedException($"Too many failed login attempts. Account locked for {LockoutMinutes} minutes.");
                    }
                }
                catch (Exception ex) { Console.WriteLine($"Redis check failed: {ex.Message}"); }
            }
            // --- End of check ---

            // Get user by email
            var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);
            
            // Verify password
            if (user == null || !_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                // --- 5. REDIS: On failed login, increment attempt counter ---
                if (_redisDb != null)
                {
                    try
                    {
                        await _redisDb.StringIncrementAsync(loginAttemptKey);
                        await _redisDb.KeyExpireAsync(loginAttemptKey, TimeSpan.FromMinutes(LockoutMinutes));
                    }
                    catch (Exception ex) { Console.WriteLine($"Redis increment failed: {ex.Message}"); }
                }
                
                // Throw the original exception
                throw new UnauthorizedException("Invalid email or password");
            }

            // --- 6. REDIS: On successful login, clear the attempt counter ---
            if (_redisDb != null)
            {
                 try
                 {
                    await _redisDb.KeyDeleteAsync(loginAttemptKey);
                 }
                 catch (Exception ex) { Console.WriteLine($"Redis key delete failed: {ex.Message}"); }
            }
            // --- End of clear ---
            
            // Create token (original logic)
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
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserId = user.UserId,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role
            };
        }
    }
}