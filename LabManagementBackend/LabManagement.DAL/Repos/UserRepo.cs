using LabManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace LabManagement.DAL.Repos
{
    public class UserRepo
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
    }
}
