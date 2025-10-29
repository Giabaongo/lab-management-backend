using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace LabManagement.DAL.Implementations;

public class ReportRepository : GenericRepository<Report>, IReportRepository
{
    public ReportRepository(LabManagementDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Report>> GetReportsByLabIdAsync(int labId)
    {
        return await _context.Reports
            .Include(r => r.Lab)
            .Include(r => r.Zone)
            .Where(r => r.LabId == labId)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetReportsByZoneIdAsync(int zoneId)
    {
        return await _context.Reports
            .Include(r => r.Lab)
            .Include(r => r.Zone)
            .Where(r => r.ZoneId == zoneId)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetReportsByUserIdAsync(int userId)
    {
        return await _context.Reports
            .Include(r => r.Lab)
            .Include(r => r.Zone)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();
    }
}
