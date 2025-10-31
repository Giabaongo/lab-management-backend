using LabManagement.DAL.Models;
using System.Linq;

namespace LabManagement.DAL.Interfaces
{
    public interface ISecurityLogRepository : IGenericRepository<SecurityLog>
    {
        IQueryable<SecurityLog> GetSecurityLogsQueryable();
    }
}
