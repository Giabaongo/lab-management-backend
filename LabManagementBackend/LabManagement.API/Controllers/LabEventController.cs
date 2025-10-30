using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Exceptions;
using LabManagement.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabManagement.API.Controllers
{
    /// <summary>
    /// Lab Event management endpoints
    /// </summary>
    [ApiController]
    [Route("api/lab-events")]
    public class LabEventController : ControllerBase
    {
        private readonly ILabEventService _labEventService;

        public LabEventController(ILabEventService labEventService)
        {
            _labEventService = labEventService;
        }

        /// <summary>
        /// Get all lab events
        /// </summary>
        /// <returns>List of lab events</returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<LabEventDTO>>>> GetAllLabEvents()
        {
            var labEvents = await _labEventService.GetAllLabEventsAsync();
            return Ok(ApiResponse<IEnumerable<LabEventDTO>>.SuccessResponse(labEvents, "Lab events retrieved successfully"));
        }

        /// <summary>
        /// Get lab events with search, sort, and pagination
        /// </summary>
        [HttpGet("paged")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<LabEventDTO>>>> GetLabEventsPaged([FromQuery] QueryParameters queryParams)
        {
            var labEvents = await _labEventService.GetLabEventsAsync(queryParams);
            return Ok(ApiResponse<PagedResult<LabEventDTO>>.SuccessResponse(labEvents, "Lab events retrieved successfully"));
        }

        /// <summary>
        /// Get lab event by ID
        /// </summary>
        /// <param name="id">Lab Event ID</param>
        /// <returns>Lab event details</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<LabEventDTO>>> GetLabEventById(int id)
        {
            var labEvent = await _labEventService.GetLabEventByIdAsync(id);
            if (labEvent == null)
            {
                throw new NotFoundException("Lab Event", id);
            }

            return Ok(ApiResponse<LabEventDTO>.SuccessResponse(labEvent, "Lab event retrieved successfully"));
        }

        /// <summary>
        /// Create a new lab event
        /// </summary>
        /// <param name="createLabEventDTO">Lab event creation data</param>
        /// <returns>Created lab event</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<LabEventDTO>>> CreateLabEvent([FromBody] CreateLabEventDTO createLabEventDTO)
        {
            if (!ModelState.IsValid)
                throw new BadRequestException("Invalid lab event data");

            var labEvent = await _labEventService.CreateLabEventAsync(createLabEventDTO);
            return CreatedAtAction(
                nameof(GetLabEventById),
                new { id = labEvent.EventId },
                ApiResponse<LabEventDTO>.SuccessResponse(labEvent, "Lab event created successfully")
            );
        }

        /// <summary>
        /// Update lab event by ID
        /// </summary>
        /// <param name="id">Lab Event ID</param>
        /// <param name="updateLabEventDTO">Lab event update data</param>
        /// <returns>Updated lab event</returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<LabEventDTO>>> UpdateLabEvent(int id, [FromBody] UpdateLabEventDTO updateLabEventDTO)
        {
            if (!ModelState.IsValid)
                throw new BadRequestException("Invalid lab event data");

            if (!await _labEventService.LabEventExistsAsync(id))
                throw new NotFoundException("Lab Event", id);

            var labEvent = await _labEventService.UpdateLabEventAsync(id, updateLabEventDTO);
            if (labEvent == null)
                throw new NotFoundException("Lab Event", id);

            return Ok(ApiResponse<LabEventDTO>.SuccessResponse(labEvent, "Lab event updated successfully"));
        }

        /// <summary>
        /// Delete lab event by ID
        /// </summary>
        /// <param name="id">Lab Event ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> DeleteLabEvent(int id)
        {
            var result = await _labEventService.DeleteLabEventAsync(id);
            if (!result)
                throw new NotFoundException("Lab Event", id);

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Lab event deleted successfully"));
        }
    }
}
