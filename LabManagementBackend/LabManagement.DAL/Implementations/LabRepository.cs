using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using System.Linq;

namespace LabManagement.DAL.Implementations
{
    public class LabRepository : GenericRepository<Lab>, ILabRepository
    {
        public LabRepository(LabManagementDbContext context) : base(context)
        {
        }

        public IQueryable<Lab> GetLabsQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }
}
