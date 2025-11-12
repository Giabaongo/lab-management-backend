using LabManagement.DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace LabManagement.DAL.Interfaces
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        IQueryable<Department> GetDepartmentsQueryable();
        Task<bool> NameExistsAsync(string name, int? excludeDepartmentId = null);
    }
}
