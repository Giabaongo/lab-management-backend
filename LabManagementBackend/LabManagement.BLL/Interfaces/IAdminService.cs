using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.Interfaces
{
    public interface IAdminService
    {
        Task<bool> UpdateUserRoleAsync(int userId, string newRole);
    }
}
