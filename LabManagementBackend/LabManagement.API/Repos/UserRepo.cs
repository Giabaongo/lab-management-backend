using LabManagement.API.Models;

namespace LabManagement.API.Repos
{
    public class UserRepo
    {
        private readonly LabManagementDbContext _context;
        public UserRepo()
        {
            _context = new LabManagementDbContext();
        }
        public User? Auth(string email, string pass)
        {
            return _context.Users
                .FirstOrDefault(u => u.Email == email && u.PasswordHash == pass);
        }
    }
}
