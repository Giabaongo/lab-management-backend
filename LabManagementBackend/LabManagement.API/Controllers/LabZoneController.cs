using LabManagement.BLL.DTOs;
using LabManagement.BLL.Implementations;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Constants;
using LabManagement.Common.Exceptions;
using LabManagement.Common.Models;
using LabManagement.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabManagement.API.Controllers
{
    /// <summary>
    /// Lab zone management endpoints
    /// </summary>
    [ApiController]
    [Route("api/lab-zones")]
    public class LabZoneController : ControllerBase
    {
        public readonly ILabZoneService _labZoneService;
        public LabZoneController(ILabZoneService labZoneService)
        {
            _labZoneService = labZoneService;
        }

        /// <summary>
        /// Get all lab zones (Admin and SchoolManager only)
        /// </summary>
        /// <returns>List of lab zones</returns>
        [HttpGet]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}, {nameof(Constant.UserRole.SecurityLab)},{nameof(Constant.UserRole.Member)},{nameof(Constant.UserRole.LabManager)}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LabZoneDTO>>>> GetAllLabZones()
        {
            var labZone = await _labZoneService.GetAllLabZonesAsync();
            if(labZone == null)
            {
                return NotFound("No lab zones found.");
            }
            return Ok(ApiResponse<IEnumerable<LabZoneDTO>>.SuccessResponse(labZone, "Lab retrieve successfully"));
        }

        /// <summary>
        /// Get Lab Zone by ID
        /// </summary>
        /// <param name="id"> LabZone ID</param>
        /// <returns>Lab Zone details</returns>
        /// <exception cref="NotFoundException"></exception>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult<ApiResponse<LabZoneDTO>>> GetLabZoneById(int id)
        {
            var labZone = await _labZoneService.GetLabZoneByIdAsync(id);
            if(labZone == null)
            {
                throw new NotFoundException("Lab Zone", id);
            }   
             
            return Ok(ApiResponse<LabZoneDTO>.SuccessResponse(labZone, "Lab Zone retrieved successfully"));
        }

        /// <summary>
        /// Create Lab Zone (Admin and SchoolManager only)
        /// </summary>
        /// <param name="createLabZoneDTO">Lab Zone creation data</param>
        /// <returns>Created Lab Zone</returns>
        [HttpPost]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")] 
        public async Task<ActionResult<ApiResponse<LabZoneDTO>>> CreateLabZone([FromBody] CreateLabZoneDTO createLabZoneDTO)
        {
            if(!ModelState.IsValid)
            {
                throw new BadRequestException("Invalid Lab Zone data");
            }

            //Check if Lab Zone with the same name already exists
            if(await _labZoneService.LabZoneNameExistsAsync(createLabZoneDTO.Name))
            {
                throw new BadRequestException($"Lab Zone with name '{createLabZoneDTO.Name}' already exists");
            }
            
            var labZone = await _labZoneService.CreateLabZoneAsync(createLabZoneDTO);
            return Ok(ApiResponse<LabZoneDTO>.SuccessResponse(labZone, "Lab Zone created successfully"));
        }

        /// <summary>
        /// Update Lab Zone (Admin and SchoolManager only)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="UpdatelabZoneDTO"></param>
        /// <returns>Updated Lab Zone</returns>
        [HttpPut]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult<ApiResponse<LabZoneDTO>>> UpdateLabZone(int id,[FromBody] UpdateLabZoneDTO UpdateLabZoneDTO)
        {
            if(!ModelState.IsValid)
            {
                throw new BadRequestException("Invalid Lab Zone data");
            }
            //Check if Lab Zone with the given ID exists
            if(!await _labZoneService.LabZoneIdExistsAsync(id))
            {
                throw new NotFoundException("Lab Zone", id);
            }
            var updatedLabZone = await _labZoneService.UpdateLabZoneAsync(id,UpdateLabZoneDTO);
            if(updatedLabZone == null)
            {
                throw new Exception("Failed to update Lab Zone");
            }
            return Ok(ApiResponse<LabZoneDTO>.SuccessResponse(updatedLabZone, "Lab Zone updated successfully"));
        }

        /// <summary>
        /// Delete Lab Zone by ID (Admin and SchoolManager only)
        /// </summary>
        /// <param name="id">Lab Zone ID</param>
        /// <returns>Success Message</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteLabZone(int id)
        {
            //Check if Lab Zone with the given ID exists
            if(!await _labZoneService.LabZoneIdExistsAsync(id))
            {
                throw new NotFoundException("Lab Zone", id);
            }
            return Ok(ApiResponse<object>.SuccessResponse(new {DeleteLabZone = id}, "Lab Zone deleted successfully"));
        }

        /// <summary>
        /// Check if Lab Zone exists
        /// </summary>
        /// <param name="id">Lab Zone ID</param>
        /// <returns>Boolean result</returns>
        [HttpGet("{id}/exists")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.LabManager)},{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult<ApiResponse<object>>> LabZoneExist(int id)
        {
            var exists = await _labZoneService.LabZoneIdExistsAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(
                new { labId = id, exists },
                exists ? "Lab Zone exists" : "Lab Zone does not exist"
            ));
        }
    }
}
