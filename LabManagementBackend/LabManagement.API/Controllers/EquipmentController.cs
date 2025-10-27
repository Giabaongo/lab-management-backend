using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Constants;
using LabManagement.Common.Exceptions;
using LabManagement.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabManagement.API.Controllers
{
    /// <summary>
    /// Equipment management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _equipmentService;
        /// <summary>
        /// Get all equipment (SchoolManager and Admin only)
        /// </summary>
        /// <returns>List of users</returns>>
        public EquipmentController(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        [HttpGet]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult> GetAllEquipment()
        {
            var equipments = await _equipmentService.GetAllEquipmentAsync();
            return Ok(ApiResponse<IEnumerable<EquipmentDTO>>.SuccessResponse(equipments, "Equipment retrieved successfully"));
        }

        /// <summary>
        /// Retrieve equipment by ID
        /// </summary>
        /// <returns>Equipment data</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.LabManager)},{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
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
            return Ok(ApiResponse<EquipmentDTO>.SuccessResponse(updatedEquipment, "Equipment updated successfully"));
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
