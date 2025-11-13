using LabManagement.DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace LabManagement.DAL.Interfaces
{
    public interface IUserDepartmentRepository : IGenericRepository<UserDepartment>
    {
        Task<int> CountByUserAsync(int userId);
        Task<UserDepartment?> GetMembershipAsync(int userId, int departmentId);
        IQueryable<UserDepartment> GetUserDepartmentsQueryable();
    }
}
