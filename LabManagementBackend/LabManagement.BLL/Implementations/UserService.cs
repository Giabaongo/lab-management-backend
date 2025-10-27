using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;

namespace LabManagement.BLL.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO?> GetUserByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            return user == null ? null : _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> CreateUserAsync(CreateUserDTO createUserDto)
        {
            var user = _mapper.Map<User>(createUserDto);
            user.PasswordHash = _passwordHasher.HashPassword(createUserDto.Password);
            user.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO?> UpdateUserAsync(int id, UpdateUserDTO updateUserDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return null;

            _mapper.Map(updateUserDto, user);

            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                user.PasswordHash = _passwordHasher.HashPassword(updateUserDto.Password);
            }

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return false;

            await _unitOfWork.Users.DeleteAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _unitOfWork.Users.ExistsAsync(u => u.UserId == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _unitOfWork.Users.EmailExistsAsync(email);
        }
    }
}
