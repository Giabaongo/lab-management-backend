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
    /// Lab Event management endpoints
    /// </summary>
    [ApiController]
    [Route("api/lab-events")]
    public class LabEventController : ControllerBase
    {
        private readonly ILabEventService _labEventService;
        private readonly IHubContext<LabEventHub> _labEventHubContext;

        public LabEventController(
            ILabEventService labEventService,
            IHubContext<LabEventHub> labEventHubContext)
        {
            _labEventService = labEventService;
            _labEventHubContext = labEventHubContext;
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
        /// <returns>Created lab event with information about cancelled items</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<LabEventCreationResultDTO>>> CreateLabEvent([FromBody] CreateLabEventDTO createLabEventDTO)
        {
            if (!ModelState.IsValid)
                throw new BadRequestException("Invalid lab event data");

            var result = await _labEventService.CreateLabEventAsync(createLabEventDTO);
            
            // Notify subscribers about new event
            await NotifyNewLabEventAsync(result.Event);
            
            // Build message based on cancellations
            var message = "Lab event created successfully";
            if (result.TotalCancelled > 0)
            {
                message += $". {result.TotalCancelled} conflicting item(s) were automatically cancelled " +
                          $"({result.CancelledBookings.Count} booking(s), {result.CancelledEvents.Count} event(s))";
            }
            
            return CreatedAtAction(
                nameof(GetLabEventById),
                new { id = result.Event.EventId },
                ApiResponse<LabEventCreationResultDTO>.SuccessResponse(result, message)
            );
        }

        private async Task NotifyNewLabEventAsync(LabEventDTO labEvent)
        {
            var eventData = new
            {
                eventId = labEvent.EventId,
                labId = labEvent.LabId,
                title = labEvent.Title,
                description = labEvent.Description,
                startTime = labEvent.StartTime,
                endTime = labEvent.EndTime,
                activityTypeId = labEvent.ActivityTypeId
            };

            // Notify all events subscribers
            await _labEventHubContext.Clients.Group(LabEventHub.GetAllEventsGroupName())
                .SendAsync("NewLabEvent", eventData);

            // Notify specific lab subscribers
            await _labEventHubContext.Clients.Group(LabEventHub.GetLabEventsGroupName(labEvent.LabId))
                .SendAsync("NewLabEvent", eventData);
        }

        /// <summary>
        /// Update lab event by ID
        /// </summary>
        /// <param name="id">Lab Event ID</param>
        /// <param name="updateLabEventDTO">Lab event update data</param>
        /// <returns>Updated lab event with information about cancelled items</returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<LabEventCreationResultDTO>>> UpdateLabEvent(int id, [FromBody] UpdateLabEventDTO updateLabEventDTO)
        {
            if (!ModelState.IsValid)
                throw new BadRequestException("Invalid lab event data");

            if (!await _labEventService.LabEventExistsAsync(id))
                throw new NotFoundException("Lab Event", id);

            var result = await _labEventService.UpdateLabEventAsync(id, updateLabEventDTO);
            if (result == null)
                throw new NotFoundException("Lab Event", id);

            // Build message based on cancellations
            var message = "Lab event updated successfully";
            if (result.TotalCancelled > 0)
            {
                message += $". {result.TotalCancelled} conflicting item(s) were automatically cancelled " +
                          $"({result.CancelledBookings.Count} booking(s), {result.CancelledEvents.Count} event(s))";
            }

            return Ok(ApiResponse<LabEventCreationResultDTO>.SuccessResponse(result, message));
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
