using LabManagement.BLL.DTOs;
using LabManagement.Common.Models;

namespace LabManagement.BLL.Interfaces;

public interface IReportService
{
    Task<IEnumerable<ReportDTO>> GetAllReportsAsync();
    Task<PagedResult<ReportDTO>> GetReportsAsync(QueryParameters queryParams);
    Task<ReportDTO?> GetReportByIdAsync(int id);
    Task<ReportDTO> CreateReportAsync(CreateReportDTO createReportDto);
    Task<ReportDTO> UpdateReportAsync(int id, UpdateReportDTO updateReportDto);
    Task<bool> DeleteReportAsync(int id);
    Task<IEnumerable<ReportDTO>> GetReportsByLabIdAsync(int labId);
    Task<IEnumerable<ReportDTO>> GetReportsByZoneIdAsync(int zoneId);
    Task<IEnumerable<ReportDTO>> GetReportsByUserIdAsync(int userId);
}
