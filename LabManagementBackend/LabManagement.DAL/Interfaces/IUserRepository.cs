using LabManagement.Common.Constants;
using LabManagement.DAL.Models;

namespace LabManagement.DAL.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetByRoleAsync(Constant.UserRole role);
        Task<bool> EmailExistsAsync(string email);
    }
}
