using LabManagement.DAL.Models;

namespace LabManagement.DAL.Interfaces;

public interface IReportRepository : IGenericRepository<Report>
{
    Task<IEnumerable<Report>> GetReportsByLabIdAsync(int labId);
    Task<IEnumerable<Report>> GetReportsByZoneIdAsync(int zoneId);
    Task<IEnumerable<Report>> GetReportsByUserIdAsync(int userId);
}
