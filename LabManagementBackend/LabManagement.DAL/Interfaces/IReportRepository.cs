using LabManagement.DAL.Models;
using System.Linq;

namespace LabManagement.DAL.Interfaces;

public interface IReportRepository : IGenericRepository<Report>
{
    IQueryable<Report> GetReportsQueryable();
    Task<IEnumerable<Report>> GetReportsByLabIdAsync(int labId);
    Task<IEnumerable<Report>> GetReportsByZoneIdAsync(int zoneId);
    Task<IEnumerable<Report>> GetReportsByUserIdAsync(int userId);
}
