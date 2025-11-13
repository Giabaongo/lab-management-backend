using LabManagement.BLL.DTOs;
using LabManagement.Common.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LabManagement.BLL.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDTO>> GetDepartmentsAsync(int? userId = null);
        Task<IEnumerable<DepartmentDTO>> GetDepartmentsForUserAsync(int userId);
        Task<DepartmentDTO?> GetDepartmentByIdAsync(int id, int? userId = null);
        Task<DepartmentDTO> CreateDepartmentAsync(CreateDepartmentDTO createDto);
        Task<DepartmentDTO?> UpdateDepartmentAsync(int id, UpdateDepartmentDTO updateDto);
        Task<bool> DeleteDepartmentAsync(int id);
        Task RegisterUserToDepartmentAsync(int userId, int departmentId, Constant.UserRole role);
        Task UnregisterUserFromDepartmentAsync(int userId, int departmentId);
        Task<bool> DepartmentExistsAsync(int id);
    }
}
