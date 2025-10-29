using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Exceptions;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;

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
