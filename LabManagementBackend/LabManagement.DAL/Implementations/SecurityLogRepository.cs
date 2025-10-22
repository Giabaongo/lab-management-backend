using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;

namespace LabManagement.DAL.Implementations
{
    public class SecurityLogRepository : GenericRepository<SecurityLog>, ISecurityLogRepository
    {
        public SecurityLogRepository(LabManagementDbContext context) : base(context)
        {
        }
    }
}
