using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Constants;
using LabManagement.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all endpoints
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Get all reports
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReportDTO>>>> GetAllReports()
    {
        var reports = await _reportService.GetAllReportsAsync();
        return Ok(ApiResponse<IEnumerable<ReportDTO>>.SuccessResponse(reports, "Reports retrieved successfully"));
    }

    /// <summary>
    /// Get report by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ReportDTO>>> GetReportById(int id)
    {
        var report = await _reportService.GetReportByIdAsync(id);
        return Ok(ApiResponse<ReportDTO>.SuccessResponse(report!, "Report retrieved successfully"));
    }

    /// <summary>
    /// Create a new report
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{nameof(Constant.UserRole.LabManager)},{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
    public async Task<ActionResult<ApiResponse<ReportDTO>>> CreateReport([FromBody] CreateReportDTO createReportDto)
    {
        var report = await _reportService.CreateReportAsync(createReportDto);
        return CreatedAtAction(nameof(GetReportById), new { id = report.ReportId }, 
            ApiResponse<ReportDTO>.SuccessResponse(report, "Report created successfully"));
    }

    /// <summary>
    /// Update an existing report
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
    public async Task<ActionResult<ApiResponse<ReportDTO>>> UpdateReport(int id, [FromBody] UpdateReportDTO updateReportDto)
    {
        var report = await _reportService.UpdateReportAsync(id, updateReportDto);
        return Ok(ApiResponse<ReportDTO>.SuccessResponse(report, "Report updated successfully"));
    }

    /// <summary>
    /// Delete a report
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteReport(int id)
    {
        var result = await _reportService.DeleteReportAsync(id);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Report deleted successfully"));
    }

    /// <summary>
    /// Get reports by Lab ID
    /// </summary>
    [HttpGet("lab/{labId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReportDTO>>>> GetReportsByLabId(int labId)
    {
        var reports = await _reportService.GetReportsByLabIdAsync(labId);
        return Ok(ApiResponse<IEnumerable<ReportDTO>>.SuccessResponse(reports, "Lab reports retrieved successfully"));
    }

    /// <summary>
    /// Get reports by Zone ID
    /// </summary>
    [HttpGet("zone/{zoneId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReportDTO>>>> GetReportsByZoneId(int zoneId)
    {
        var reports = await _reportService.GetReportsByZoneIdAsync(zoneId);
        return Ok(ApiResponse<IEnumerable<ReportDTO>>.SuccessResponse(reports, "Zone reports retrieved successfully"));
    }

    /// <summary>
    /// Get reports by User ID
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReportDTO>>>> GetReportsByUserId(int userId)
    {
        var reports = await _reportService.GetReportsByUserIdAsync(userId);
        return Ok(ApiResponse<IEnumerable<ReportDTO>>.SuccessResponse(reports, "User reports retrieved successfully"));
    }
}
