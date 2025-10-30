using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using System.Linq;

namespace LabManagement.DAL.Implementations
{
    public class SecurityLogRepository : GenericRepository<SecurityLog>, ISecurityLogRepository
    {
        public SecurityLogRepository(LabManagementDbContext context) : base(context)
        {
        }

        public IQueryable<SecurityLog> GetSecurityLogsQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }
}
