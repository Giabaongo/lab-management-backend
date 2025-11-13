using LabManagement.API.Hubs;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Constants;
using LabManagement.Common.Exceptions;
using LabManagement.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LabManagement.API.Controllers
{
    /// <summary>
    /// Equipment management endpoints
    /// </summary>
    [ApiController]
    [Route("api/equipments")] // Changed to plural
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _equipmentService;
        private readonly IHubContext<EquipmentHub> _equipmentHubContext;
        
        /// <summary>
        /// Get all equipment (SchoolManager and Admin only)
        /// </summary>
        /// <returns>List of users</returns>>
        public EquipmentController(
            IEquipmentService equipmentService,
            IHubContext<EquipmentHub> equipmentHubContext)
        {
            _equipmentService = equipmentService;
            _equipmentHubContext = equipmentHubContext;
        }

        [HttpGet]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)},{nameof(Constant.UserRole.SecurityLab)},{nameof(Constant.UserRole.Member)},{nameof(Constant.UserRole.LabManager)}")]
        public async Task<ActionResult> GetAllEquipment()
        {
            var equipments = await _equipmentService.GetAllEquipmentAsync();
            return Ok(ApiResponse<IEnumerable<EquipmentDTO>>.SuccessResponse(equipments, "Equipment retrieved successfully"));
        }

        /// <summary>
        /// Get equipment with search, sort, and pagination
        /// </summary>
        [HttpGet("paged")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)},{nameof(Constant.UserRole.SecurityLab)},{nameof(Constant.UserRole.Member)},{nameof(Constant.UserRole.LabManager)}")]
        public async Task<ActionResult<ApiResponse<PagedResult<EquipmentDTO>>>> GetEquipmentPaged([FromQuery] QueryParameters queryParams)
        {
            var equipments = await _equipmentService.GetEquipmentAsync(queryParams);
            return Ok(ApiResponse<PagedResult<EquipmentDTO>>.SuccessResponse(equipments, "Equipment retrieved successfully"));
        }

        /// <summary>
        /// Retrieve equipment by ID
        /// </summary>
        /// <returns>Equipment data</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.LabManager)},{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)},{nameof(Constant.UserRole.SecurityLab)},{nameof(Constant.UserRole.Member)}")]
        public async Task<ActionResult> GetEquipmentById(int id)
        {
            var equipment = await _equipmentService.GetEquipmentByIdAsync(id);
            if (equipment == null)
                throw new NotFoundException("Equipment not found");
            return Ok(ApiResponse<EquipmentDTO>.SuccessResponse(equipment, "Equipment retrieved successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult> UpdateEquipment(int id, UpdateEquipmentDTO updateEquipmentDTO)
        {
            if(!ModelState.IsValid)
                throw new BadRequestException("Invalid equipment data");

            //check for equipment existence with given id exists
            var updatedEquipment = await _equipmentService.UpdateEquipmentAsync(id, updateEquipmentDTO);
            if (updatedEquipment == null)
                throw new NotFoundException("Equipment not found");
            
            // Notify if status changed to broken or maintenance
            if (updatedEquipment.Status == 2 || updatedEquipment.Status == 3) // Broken or Maintenance
            {
                await NotifyEquipmentStatusChangeAsync(updatedEquipment);
            }
            
            return Ok(ApiResponse<EquipmentDTO>.SuccessResponse(updatedEquipment, "Equipment updated successfully"));
        }

        private async Task NotifyEquipmentStatusChangeAsync(EquipmentDTO equipment)
        {
            // Notify all managers
            await _equipmentHubContext.Clients.Group(EquipmentHub.GetAllManagersGroupName())
                .SendAsync("EquipmentStatusChanged", new
                {
                    equipmentId = equipment.EquipmentId,
                    equipmentName = equipment.Name,
                    equipmentCode = equipment.Code,
                    labId = equipment.LabId,
                    status = equipment.Status,
                    statusText = equipment.Status == 2 ? "Broken" : "Under Maintenance",
                    timestamp = DateTime.UtcNow
                });

            // Also notify specific lab managers
            await _equipmentHubContext.Clients.Group(EquipmentHub.GetLabManagerGroupName(equipment.LabId))
                .SendAsync("EquipmentStatusChanged", new
                {
                    equipmentId = equipment.EquipmentId,
                    equipmentName = equipment.Name,
                    equipmentCode = equipment.Code,
                    labId = equipment.LabId,
                    status = equipment.Status,
                    statusText = equipment.Status == 2 ? "Broken" : "Under Maintenance",
                    timestamp = DateTime.UtcNow
                });
        }

        /// <summary>
        /// Delete equipment by ID
        /// </summary>
        /// <returns>Succesfull message</returns>
        /// <exception cref="NotFoundException"></exception>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult> DeleteEquipment(int id)
        {
            var isDeleted = await _equipmentService.DeleteEquipmentAsync(id);
            if (!isDeleted)
                throw new NotFoundException("Equipment not found");
            return Ok(ApiResponse<string>.SuccessResponse("","Equipment deleted successfully"));
        }

        /// <summary>
        /// Check if equipment code exists
        /// </summary>
        /// <returns>Boolean result</returns>
        [HttpGet("code-exist")]
        public async Task<ActionResult> CheckEquipmentCodeExists([FromQuery] string code)
        {
            var exists = await _equipmentService.EquipmentCodeExistsAsync(code);
            return Ok(ApiResponse<bool>.SuccessResponse(exists, "Equipment code existence checked successfully"));
        }

        /// <summary>
        /// Check if equipment status exists
        /// </summary>
        /// <returns>Boolean result</returns>
        [HttpGet("status-exist")]
        public async Task<ActionResult> CheckEquipmentStatusExists([FromQuery] int status)
        {
            var exists = await _equipmentService.EquipmentStatusExistsAsync(status);
            return Ok(ApiResponse<bool>.SuccessResponse(exists, "Equipment status existence checked successfully"));
        }
    }
}
