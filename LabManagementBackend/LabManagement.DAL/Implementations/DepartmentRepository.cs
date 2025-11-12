using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LabManagement.DAL.Implementations
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(LabManagementDbContext context) : base(context)
        {
        }

        public IQueryable<Department> GetDepartmentsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeDepartmentId = null)
        {
            return await _dbSet.AnyAsync(d =>
                d.Name == name &&
                (!excludeDepartmentId.HasValue || d.DepartmentId != excludeDepartmentId.Value));
        }
    }
}
