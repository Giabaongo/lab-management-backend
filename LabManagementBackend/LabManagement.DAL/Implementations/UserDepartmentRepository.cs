using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LabManagement.DAL.Implementations
{
    public class UserDepartmentRepository : GenericRepository<UserDepartment>, IUserDepartmentRepository
    {
        public UserDepartmentRepository(LabManagementDbContext context) : base(context)
        {
        }

        public async Task<int> CountByUserAsync(int userId)
        {
            return await _dbSet.CountAsync(ud => ud.UserId == userId);
        }

        public IQueryable<UserDepartment> GetUserDepartmentsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<UserDepartment?> GetMembershipAsync(int userId, int departmentId)
        {
            return await _dbSet.FirstOrDefaultAsync(ud =>
                ud.UserId == userId && ud.DepartmentId == departmentId);
        }
    }
}
