using LabManagement.BLL.Interfaces;
using LabManagement.DAL.Repos;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserRepo _userRepo;
        public AdminService(UserRepo userRepo)
        {
            _userRepo = userRepo;
        }
        public async Task<bool> UpdateUserRoleAsync(int userId, string newRole)
        {
            var getUser = await _userRepo.GetByIdAsync(userId);
            if (getUser != null)
            {
                getUser.Role = 
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
    }
}
