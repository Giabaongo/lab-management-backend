using LabManagement.BLL.Interfaces;
using LabManagement.Common.Constants;
using LabManagement.DAL.Interfaces;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _IUserRepo;
        public AdminService(IUserRepository userRepository)
        {
            _IUserRepo = userRepository;
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, Constant.UserRole newRole)
        {
            var getUser = await _IUserRepo.GetByIdAsync(userId);
            if (getUser == null)
            {
                return false;
            }

            getUser.Role = (int)newRole;
            await _IUserRepo.UpdateAsync(getUser);

            return true;
        }
    }
}
