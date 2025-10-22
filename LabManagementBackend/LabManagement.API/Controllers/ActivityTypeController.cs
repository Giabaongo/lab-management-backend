using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Exceptions;
using LabManagement.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabManagement.API.Controllers
{
    /// <summary>
    /// Activity Type management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityTypeController : ControllerBase
    {
        private readonly IActivityTypeService _activityTypeService;

        public ActivityTypeController(IActivityTypeService activityTypeService)
        {
            _activityTypeService = activityTypeService;
        }

        /// <summary>
        /// Get all activity types
        /// </summary>
        /// <returns>List of activity types</returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<ActivityTypeDTO>>>> GetAllActivityTypes()
        {
            var activityTypes = await _activityTypeService.GetAllActivityTypesAsync();
            return Ok(ApiResponse<IEnumerable<ActivityTypeDTO>>.SuccessResponse(activityTypes, "Activity types retrieved successfully"));
        }

        /// <summary>
        /// Get activity type by ID
        /// </summary>
        /// <param name="id">Activity Type ID</param>
        /// <returns>Activity type details</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ActivityTypeDTO>>> GetActivityTypeById(int id)
        {
            var activityType = await _activityTypeService.GetActivityTypeByIdAsync(id);
            if (activityType == null)
            {
                throw new NotFoundException("Activity Type", id);
            }

            return Ok(ApiResponse<ActivityTypeDTO>.SuccessResponse(activityType, "Activity type retrieved successfully"));
        }

        /// <summary>
        /// Create a new activity type
        /// </summary>
        /// <param name="createActivityTypeDTO">Activity type creation data</param>
        /// <returns>Created activity type</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ActivityTypeDTO>>> CreateActivityType([FromBody] CreateActivityTypeDTO createActivityTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BadRequestException("Invalid activity type data");

            var activityType = await _activityTypeService.CreateActivityTypeAsync(createActivityTypeDTO);
            return CreatedAtAction(
                nameof(GetActivityTypeById),
                new { id = activityType.ActivityTypeId },
                ApiResponse<ActivityTypeDTO>.SuccessResponse(activityType, "Activity type created successfully")
            );
        }

        /// <summary>
        /// Update activity type by ID
        /// </summary>
        /// <param name="id">Activity Type ID</param>
        /// <param name="updateActivityTypeDTO">Activity type update data</param>
        /// <returns>Updated activity type</returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ActivityTypeDTO>>> UpdateActivityType(int id, [FromBody] UpdateActivityTypeDTO updateActivityTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BadRequestException("Invalid activity type data");

            if (!await _activityTypeService.ActivityTypeExistsAsync(id))
                throw new NotFoundException("Activity Type", id);

            var activityType = await _activityTypeService.UpdateActivityTypeAsync(id, updateActivityTypeDTO);
            if (activityType == null)
                throw new NotFoundException("Activity Type", id);

            return Ok(ApiResponse<ActivityTypeDTO>.SuccessResponse(activityType, "Activity type updated successfully"));
        }

        /// <summary>
        /// Delete activity type by ID
        /// </summary>
        /// <param name="id">Activity Type ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> DeleteActivityType(int id)
        {
            var result = await _activityTypeService.DeleteActivityTypeAsync(id);
            if (!result)
                throw new NotFoundException("Activity Type", id);

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Activity type deleted successfully"));
        }
    }
}
