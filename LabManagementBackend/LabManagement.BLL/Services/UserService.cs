using LabManagement.BLL.DTOs;
using LabManagement.DAL.Models;
using LabManagement.DAL.Repos;

namespace LabManagement.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepo _userRepo;

        public UserService()
        {
            _userRepo = new UserRepo();
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllAsync();
            return users.Select(u => new UserDTO
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            });
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return null;

            return new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserDTO?> GetUserByEmailAsync(string email)
        {
            var users = await _userRepo.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email == email);
            if (user == null) return null;

            return new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserDTO> CreateUserAsync(CreateUserDTO createUserDto)
        {
            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                PasswordHash = createUserDto.Password, // Store password as plain text for now
                Role = createUserDto.Role,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.AddASync(user);

            return new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserDTO?> UpdateUserAsync(int id, UpdateUserDTO updateUserDto)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return null;

            if (!string.IsNullOrEmpty(updateUserDto.Name))
                user.Name = updateUserDto.Name;

            if (!string.IsNullOrEmpty(updateUserDto.Email))
                user.Email = updateUserDto.Email;

            if (!string.IsNullOrEmpty(updateUserDto.Password))
                user.PasswordHash = updateUserDto.Password; // Store password as plain text for now

            if (updateUserDto.Role.HasValue)
                user.Role = updateUserDto.Role.Value;

            await _userRepo.UpdateAsync(user);

            return new UserDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return false;

            await _userRepo.DeleteAsync(id);
            return true;
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            return user != null;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var users = await _userRepo.GetAllAsync();
            return users.Any(u => u.Email == email);
        }
    }
}