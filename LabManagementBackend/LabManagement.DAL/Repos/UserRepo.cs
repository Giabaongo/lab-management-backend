using LabManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace LabManagement.DAL.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly LabManagementDbContext _context;
        public UserRepo()
        {
            _context = new LabManagementDbContext();
        }
        public async Task<User?> Auth(string? email, string? pass)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == pass);
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task AddASync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            await _context.SaveChangesAsync();
        }
    }
}
