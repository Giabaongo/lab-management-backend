using LabManagement.API.Hubs;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Exceptions;
using LabManagement.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LabManagement.API.Controllers
{
    /// <summary>
    /// Security Log management endpoints
    /// </summary>
    [ApiController]
    [Route("api/security-logs")]
    public class SecurityLogController : ControllerBase
    {
        private readonly ISecurityLogService _securityLogService;
        private readonly IHubContext<SecurityLogHub> _securityLogHubContext;

        public SecurityLogController(
            ISecurityLogService securityLogService,
            IHubContext<SecurityLogHub> securityLogHubContext)
        {
            _securityLogService = securityLogService;
            _securityLogHubContext = securityLogHubContext;
        }

        /// <summary>
        /// Get all security logs
        /// </summary>
        /// <returns>List of security logs</returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<SecurityLogDTO>>>> GetAllSecurityLogs()
        {
            var securityLogs = await _securityLogService.GetAllSecurityLogsAsync();
            return Ok(ApiResponse<IEnumerable<SecurityLogDTO>>.SuccessResponse(securityLogs, "Security logs retrieved successfully"));
        }

        /// <summary>
        /// Get security logs with search, sort, and pagination
        /// </summary>
        [HttpGet("paged")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<SecurityLogDTO>>>> GetSecurityLogsPaged([FromQuery] QueryParameters queryParams)
        {
            var securityLogs = await _securityLogService.GetSecurityLogsAsync(queryParams);
            return Ok(ApiResponse<PagedResult<SecurityLogDTO>>.SuccessResponse(securityLogs, "Security logs retrieved successfully"));
        }

        /// <summary>
        /// Get security log by ID
        /// </summary>
        /// <param name="id">Security Log ID</param>
        /// <returns>Security log details</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<SecurityLogDTO>>> GetSecurityLogById(int id)
        {
            var securityLog = await _securityLogService.GetSecurityLogByIdAsync(id);
            if (securityLog == null)
            {
                throw new NotFoundException("Security Log", id);
            }

            return Ok(ApiResponse<SecurityLogDTO>.SuccessResponse(securityLog, "Security log retrieved successfully"));
        }

        /// <summary>
        /// Create a new security log
        /// </summary>
        /// <param name="createSecurityLogDTO">Security log creation data</param>
        /// <returns>Created security log</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<SecurityLogDTO>>> CreateSecurityLog([FromBody] CreateSecurityLogDTO createSecurityLogDTO)
        {
            if (!ModelState.IsValid)
                throw new BadRequestException("Invalid security log data");

            var securityLog = await _securityLogService.CreateSecurityLogAsync(createSecurityLogDTO);
            
            // Send real-time security alert
            await NotifySecurityAlertAsync(securityLog);
            
            return CreatedAtAction(
                nameof(GetSecurityLogById),
                new { id = securityLog.LogId },
                ApiResponse<SecurityLogDTO>.SuccessResponse(securityLog, "Security log created successfully")
            );
        }

        /// <summary>
        /// Update security log by ID
        /// </summary>
        /// <param name="id">Security Log ID</param>
        /// <param name="updateSecurityLogDTO">Security log update data</param>
        /// <returns>Updated security log</returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<SecurityLogDTO>>> UpdateSecurityLog(int id, [FromBody] UpdateSecurityLogDTO updateSecurityLogDTO)
        {
            if (!ModelState.IsValid)
                throw new BadRequestException("Invalid security log data");

            if (!await _securityLogService.SecurityLogExistsAsync(id))
                throw new NotFoundException("Security Log", id);

            var securityLog = await _securityLogService.UpdateSecurityLogAsync(id, updateSecurityLogDTO);
            if (securityLog == null)
                throw new NotFoundException("Security Log", id);

            return Ok(ApiResponse<SecurityLogDTO>.SuccessResponse(securityLog, "Security log updated successfully"));
        }

        /// <summary>
        /// Delete security log by ID
        /// </summary>
        /// <param name="id">Security Log ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> DeleteSecurityLog(int id)
        {
            var result = await _securityLogService.DeleteSecurityLogAsync(id);
            if (!result)
                throw new NotFoundException("Security Log", id);

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Security log deleted successfully"));
        }

        private async Task NotifySecurityAlertAsync(SecurityLogDTO securityLog)
        {
            var alertData = new
            {
                logId = securityLog.LogId,
                eventId = securityLog.EventId,
                securityId = securityLog.SecurityId,
                actionType = securityLog.ActionType,
                notes = securityLog.Notes,
                photoUrl = securityLog.PhotoUrl,
                loggedAt = securityLog.LoggedAt,
                severity = securityLog.ActionType == 2 ? "Critical" : "Warning" // Adjust based on your action types
            };

            // Notify security team (Admin, SchoolManager, SecurityLab)
            await _securityLogHubContext.Clients.Group(SecurityLogHub.GetSecurityTeamGroupName())
                .SendAsync("SecurityAlert", alertData);
        }
    }
}
