using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Exceptions;
using LabManagement.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabManagement.API.Controllers
{
    /// <summary>
    /// Security Log management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SecurityLogController : ControllerBase
    {
        private readonly ISecurityLogService _securityLogService;

        public SecurityLogController(ISecurityLogService securityLogService)
        {
            _securityLogService = securityLogService;
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
    }
}
