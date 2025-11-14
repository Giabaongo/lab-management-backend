using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Constants;
using LabManagement.Common.Exceptions;
using LabManagement.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LabManagement.API.Controllers
{
    [ApiController]
    [Route("api/departments")]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
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

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentDTO>>>> GetDepartments()
        {
            var (userId, _) = GetRequesterContext();
            var departments = await _departmentService.GetDepartmentsAsync(userId);
            return Ok(ApiResponse<IEnumerable<DepartmentDTO>>.SuccessResponse(departments, "Departments retrieved successfully"));
        }

        [HttpGet("my")]
        public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentDTO>>>> GetMyDepartments()
        {
            var (userId, _) = GetRequesterContext();
            var departments = await _departmentService.GetDepartmentsForUserAsync(userId);
            return Ok(ApiResponse<IEnumerable<DepartmentDTO>>.SuccessResponse(departments, "Member departments retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<DepartmentDTO>>> GetDepartmentById(int id)
        {
            var (userId, _) = GetRequesterContext();
            var department = await _departmentService.GetDepartmentByIdAsync(id, userId);
            if (department == null)
            {
                throw new NotFoundException("Department", id);
            }

            return Ok(ApiResponse<DepartmentDTO>.SuccessResponse(department, "Department retrieved successfully"));
        }

        [HttpPost]
        [Authorize(Roles = $"{nameof(Constant.UserRole.Admin)},{nameof(Constant.UserRole.SchoolManager)}")]
        public async Task<ActionResult<ApiResponse<DepartmentDTO>>> CreateDepartment([FromBody] CreateDepartmentDTO createDepartmentDto)
        {
            if (!ModelState.IsValid)
            {
                throw new BadRequestException("Invalid department data");
            }

            var department = await _departmentService.CreateDepartmentAsync(createDepartmentDto);
            return CreatedAtAction(
                nameof(GetDepartmentById),
                new { id = department.DepartmentId },
                ApiResponse<DepartmentDTO>.SuccessResponse(department, "Department created successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.Admin)},{nameof(Constant.UserRole.SchoolManager)}")]
        public async Task<ActionResult<ApiResponse<DepartmentDTO>>> UpdateDepartment(int id, [FromBody] UpdateDepartmentDTO updateDepartmentDto)
        {
            if (!ModelState.IsValid)
            {
                throw new BadRequestException("Invalid department data");
            }

            var department = await _departmentService.UpdateDepartmentAsync(id, updateDepartmentDto);
            if (department == null)
            {
                throw new NotFoundException("Department", id);
            }

            return Ok(ApiResponse<DepartmentDTO>.SuccessResponse(department, "Department updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(Constant.UserRole.Admin))]
        public async Task<ActionResult<ApiResponse<object>>> DeleteDepartment(int id)
        {
            var deleted = await _departmentService.DeleteDepartmentAsync(id);
            if (!deleted)
            {
                throw new NotFoundException("Department", id);
            }

            return Ok(ApiResponse<object>.SuccessResponse(new { departmentId = id }, "Department deleted successfully"));
        }

        [HttpPost("{id}/register")]
        [Authorize(Roles = nameof(Constant.UserRole.Member))]
        public async Task<ActionResult<ApiResponse<object>>> RegisterToDepartment(int id)
        {
            var (userId, role) = GetRequesterContext();
            await _departmentService.RegisterUserToDepartmentAsync(userId, id, role);

            return Ok(ApiResponse<object>.SuccessResponse(new { departmentId = id }, "Registered to department successfully"));
        }

        [HttpDelete("{id}/register")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.Member)},{nameof(Constant.UserRole.Admin)},{nameof(Constant.UserRole.SchoolManager)}")]
        public async Task<ActionResult<ApiResponse<object>>> UnregisterFromDepartment(int id, [FromQuery] int? userIdOverride = null)
        {
            var (userId, role) = GetRequesterContext();
            var targetUserId = userId;

            if (userIdOverride.HasValue && (role == Constant.UserRole.Admin || role == Constant.UserRole.SchoolManager))
            {
                targetUserId = userIdOverride.Value;
            }

            await _departmentService.UnregisterUserFromDepartmentAsync(targetUserId, id);
            return Ok(ApiResponse<object>.SuccessResponse(new { departmentId = id, userId = targetUserId }, "Unregistered from department successfully"));
        }
    }
}
