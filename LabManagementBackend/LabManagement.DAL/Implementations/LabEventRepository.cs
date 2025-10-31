using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using System.Linq;

namespace LabManagement.DAL.Implementations
{
    public class LabEventRepository : GenericRepository<LabEvent>, ILabEventRepository
    {
        public LabEventRepository(LabManagementDbContext context) : base(context)
        {
        }

        public IQueryable<LabEvent> GetLabEventsQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }
}
