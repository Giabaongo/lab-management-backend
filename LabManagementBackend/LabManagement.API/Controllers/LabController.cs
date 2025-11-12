using System;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Implementations;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Constants;
using LabManagement.Common.Exceptions;
using LabManagement.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LabManagement.API.Controllers
{
    /// <summary>
    /// Lab management endpoints
    /// </summary>
    [ApiController]
    [Route("api/labs")]
    public class LabController : ControllerBase
    {
        private readonly ILabService _labService;

        public LabController(ILabService labService)
        {
            _labService = labService;
        }

        private (int userId, Constant.UserRole role) GetRequesterContext()
        {
            var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirst(ClaimTypes.Role) ?? User.FindFirst("Role");

            if (userIdClaim == null || roleClaim == null)
            {
                throw new UnauthorizedException("Missing authentication context");
            }

            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedException("Invalid user identifier");
            }

            if (!Enum.TryParse(roleClaim.Value, out Constant.UserRole role))
            {
                throw new UnauthorizedException("Invalid user role");
            }

            return (userId, role);
        }

        /// <summary>
        /// Get all Labs(Admin and School manager only)
        /// </summary>
        /// <returns>List of labs</returns>
        [HttpGet]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}, {nameof(Constant.UserRole.SecurityLab)},{nameof(Constant.UserRole.Member)},{nameof(Constant.UserRole.LabManager)}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LabDTO>>>> GetAllLabs()
        {
            var (userId, role) = GetRequesterContext();
            var labs = await _labService.GetAllLabsAsync(userId, role);
            return Ok(ApiResponse<IEnumerable<LabDTO>>.SuccessResponse(labs, "Labs retrieved successfully"));
        }

        /// <summary>
        /// Get labs with search, sort, and pagination
        /// </summary>
        [HttpGet("paged")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}, {nameof(Constant.UserRole.SecurityLab)},{nameof(Constant.UserRole.Member)},{nameof(Constant.UserRole.LabManager)}")]
        public async Task<ActionResult<ApiResponse<PagedResult<LabDTO>>>> GetLabsPaged([FromQuery] QueryParameters queryParams)
        {
            var (userId, role) = GetRequesterContext();
            var labs = await _labService.GetLabsAsync(queryParams, userId, role);
            return Ok(ApiResponse<PagedResult<LabDTO>>.SuccessResponse(labs, "Labs retrieved successfully"));
        }

        /// <summary>
        /// Get Lab by ID 
        /// </summary>
        /// <param name="id">Lab ID</param>
        /// <returns>Lab details</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult<ApiResponse<LabDTO>>> GetLabById(int id)
        {
            var lab = await _labService.GetLabByIdAsync(id);
            if (lab == null)
            {
                throw new NotFoundException("Lab", id);
            }

            return Ok(ApiResponse<LabDTO>.SuccessResponse(lab, "Lab retrieved successfully"));
        }

        /// <summary>
        /// Create a new lab (Admin only)
        /// </summary>
        /// <param name="createLabDTO">Lab creation data</param>
        /// <returns>Created lab</returns>
        /// <exception cref="BadRequestException"></exception>
        [HttpPost]
        [Authorize(Roles = nameof(Constant.UserRole.Admin))]
        public async Task<ActionResult<ApiResponse<LabDTO>>> CreateLab([FromBody] CreateLabDTO createLabDTO)
        {
            if (!ModelState.IsValid) throw new BadRequestException("Invalid lab data");

            //check if lab already exists (by name)
            if (await _labService.LabExistsAsync(createLabDTO.name))
            {
                throw new BadRequestException($"Lab '{createLabDTO.name}' already exists");
            }

            var lab = await _labService.CreateLabAsync(createLabDTO);
            return CreatedAtAction(
                nameof(GetLabById),
                new { id = lab.labId },
                ApiResponse<LabDTO>.SuccessResponse(lab, "Lab created successfully")
                );
        }

        /// <summary>
        /// Update Lab by ID (Admin and SchoolManager only)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateLabDTO">Lab Id</param>
        /// <returns>Update Lab</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult<ApiResponse<LabDTO>>> UpdateLab(string name, [FromBody] UpdateLabDTO updateLabDTO)
        {
            if (!ModelState.IsValid)
                throw new BadRequestException("Invalid lab data");

            // Check if lab exists
            if (!await _labService.LabExistsAsync(name))
                throw new NotFoundException("Lab", name);

            var lab = await _labService.UpdateLabAsync(updateLabDTO, name);

            if (lab == null)
                throw new NotFoundException("Lab", name);

            return Ok(ApiResponse<LabDTO>.SuccessResponse(lab, "Lab updated successfully"));
        }

        /// <summary>
        /// Delete Lab by ID (Admin and SchoolManager only)
        /// </summary>
        /// <param name="id">Lab id</param>
        /// <returns>Delete Lab</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(Constant.UserRole.Admin))]
        public async Task<ActionResult<ApiResponse<object>>> DeleteLab(int id)
        {
            var result = await _labService.DeleteLabAsync(id);

            if (!result)
                throw new NotFoundException("Lab", id);

            return Ok(ApiResponse<object>.SuccessResponse(
                new { deleteLabId = id },
                "Lab deleted successfully"
            ));
        }

        /// <summary>
        /// Check if lab exists
        /// </summary>
        /// <param name="id">Lab ID</param>
        /// <returns>Boolean result</returns>
        [HttpGet("{id}/exists")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.LabManager)},{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult<ApiResponse<object>>> LabExists(string name)
        {
            var exists = await _labService.LabExistsAsync(name);
            return Ok(ApiResponse<object>.SuccessResponse(
                new { labName = name, exists },
                exists ? "Lab exists" : "Lab does not exist"
            ));
        }
    }
}
