using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Exceptions;
using LabManagement.Common.Extensions;
using LabManagement.Common.Models;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using System.Collections.Generic;
using System.Linq;

namespace LabManagement.BLL.Implementations;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReportDTO>> GetAllReportsAsync()
    {
        var reports = await _unitOfWork.Reports.GetAllAsync();
        return _mapper.Map<IEnumerable<ReportDTO>>(reports);
    }

    public async Task<PagedResult<ReportDTO>> GetReportsAsync(QueryParameters queryParams)
    {
        var query = _unitOfWork.Reports.GetReportsQueryable();

        if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
        {
            var term = queryParams.SearchTerm.ToLower();
            query = query.Where(r =>
                r.ReportType.ToLower().Contains(term) ||
                (r.Content != null && r.Content.ToLower().Contains(term)) ||
                (r.PhotoUrl != null && r.PhotoUrl.ToLower().Contains(term)) ||
                (r.Lab != null && r.Lab.Name.ToLower().Contains(term)) ||
                (r.Zone != null && r.Zone.Name.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(queryParams.SortBy))
        {
            query = queryParams.SortBy.ToLower() switch
            {
                "reporttype" => queryParams.IsDescending ? query.OrderByDescending(r => r.ReportType) : query.OrderBy(r => r.ReportType),
                "generatedat" => queryParams.IsDescending ? query.OrderByDescending(r => r.GeneratedAt) : query.OrderBy(r => r.GeneratedAt),
                "labid" => queryParams.IsDescending ? query.OrderByDescending(r => r.LabId) : query.OrderBy(r => r.LabId),
                "zoneid" => queryParams.IsDescending ? query.OrderByDescending(r => r.ZoneId) : query.OrderBy(r => r.ZoneId),
                "userid" => queryParams.IsDescending ? query.OrderByDescending(r => r.UserId) : query.OrderBy(r => r.UserId),
                _ => query.OrderBy(r => r.ReportId)
            };
        }
        else
        {
            query = query.OrderBy(r => r.ReportId);
        }

        var pagedReports = await query.ToPagedResultAsync(queryParams.PageNumber, queryParams.PageSize);

        return new PagedResult<ReportDTO>
        {
            Items = _mapper.Map<IEnumerable<ReportDTO>>(pagedReports.Items),
            PageNumber = pagedReports.PageNumber,
            PageSize = pagedReports.PageSize,
            TotalCount = pagedReports.TotalCount
        };
    }

    public async Task<ReportDTO?> GetReportByIdAsync(int id)
    {
        var report = await _unitOfWork.Reports.GetByIdAsync(id);
        if (report == null)
        {
            throw new NotFoundException($"Report with ID {id} not found");
        }
        return _mapper.Map<ReportDTO>(report);
    }

    public async Task<ReportDTO> CreateReportAsync(CreateReportDTO createReportDto)
    {
        // Validate Lab exists if LabId is provided
        if (createReportDto.LabId.HasValue)
        {
            var lab = await _unitOfWork.Labs.GetByIdAsync(createReportDto.LabId.Value);
            if (lab == null)
            {
                throw new NotFoundException($"Lab with ID {createReportDto.LabId} not found");
            }
        }

        // Validate Zone exists if ZoneId is provided
        if (createReportDto.ZoneId.HasValue)
        {
            var zone = await _unitOfWork.LabZones.GetByIdAsync(createReportDto.ZoneId.Value);
            if (zone == null)
            {
                throw new NotFoundException($"Zone with ID {createReportDto.ZoneId} not found");
            }
        }

        var report = _mapper.Map<Report>(createReportDto);
        await _unitOfWork.Reports.AddAsync(report);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ReportDTO>(report);
    }

    public async Task<ReportDTO> UpdateReportAsync(int id, UpdateReportDTO updateReportDto)
    {
        var report = await _unitOfWork.Reports.GetByIdAsync(id);
        if (report == null)
        {
            throw new NotFoundException($"Report with ID {id} not found");
        }

        // Validate Lab exists if LabId is provided
        if (updateReportDto.LabId.HasValue)
        {
            var lab = await _unitOfWork.Labs.GetByIdAsync(updateReportDto.LabId.Value);
            if (lab == null)
            {
                throw new NotFoundException($"Lab with ID {updateReportDto.LabId} not found");
            }
        }

        // Validate Zone exists if ZoneId is provided
        if (updateReportDto.ZoneId.HasValue)
        {
            var zone = await _unitOfWork.LabZones.GetByIdAsync(updateReportDto.ZoneId.Value);
            if (zone == null)
            {
                throw new NotFoundException($"Zone with ID {updateReportDto.ZoneId} not found");
            }
        }

        _mapper.Map(updateReportDto, report);
        await _unitOfWork.Reports.UpdateAsync(report);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ReportDTO>(report);
    }

    public async Task<bool> DeleteReportAsync(int id)
    {
        var report = await _unitOfWork.Reports.GetByIdAsync(id);
        if (report == null)
        {
            throw new NotFoundException($"Report with ID {id} not found");
        }

        await _unitOfWork.Reports.DeleteAsync(report);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ReportDTO>> GetReportsByLabIdAsync(int labId)
    {
        var reports = await _unitOfWork.Reports.GetReportsByLabIdAsync(labId);
        return _mapper.Map<IEnumerable<ReportDTO>>(reports);
    }

    public async Task<IEnumerable<ReportDTO>> GetReportsByZoneIdAsync(int zoneId)
    {
        var reports = await _unitOfWork.Reports.GetReportsByZoneIdAsync(zoneId);
        return _mapper.Map<IEnumerable<ReportDTO>>(reports);
    }

    public async Task<IEnumerable<ReportDTO>> GetReportsByUserIdAsync(int userId)
    {
        var reports = await _unitOfWork.Reports.GetReportsByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<ReportDTO>>(reports);
    }
}
