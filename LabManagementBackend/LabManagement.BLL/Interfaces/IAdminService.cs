using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using LabManagement.Common.Constants;

namespace LabManagement.BLL.Interfaces
{
    public interface IAdminService
    {
        Task<bool> UpdateUserRoleAsync(int userId, LabManagement.Common.Constants.Constant.UserRole newRole);
    }
}

