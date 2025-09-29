using LabManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.DAL.Repos
{
    public interface IUserRepo
    {
        Task<User?> Auth(string? email, string? pass);
        Task<User?> GetByIdAsync(int userId);
        Task<List<User>> GetAllAsync();
        Task AddASync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int userId);

    }
}
