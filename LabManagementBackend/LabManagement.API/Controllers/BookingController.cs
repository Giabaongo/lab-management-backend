using System;
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
    /// <summary>
    /// Booking management endpoints
    /// </summary>
    [ApiController]
    [Route("api/bookings")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
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
        /// Get all bookings
        /// </summary>
        /// <returns>List of bookings</returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<BookingDTO>>>> GetAllBookings()
        {
            var (userId, role) = GetRequesterContext();
            var bookings = await _bookingService.GetAllBookingsAsync(userId, role);
            return Ok(ApiResponse<IEnumerable<BookingDTO>>.SuccessResponse(bookings, "Bookings retrieved successfully"));
        }

        /// <summary>
        /// Get bookings with search, sort, and pagination
        /// </summary>
        [HttpGet("paged")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<BookingDTO>>>> GetBookingsPaged([FromQuery] QueryParameters queryParams)
        {
            var (userId, role) = GetRequesterContext();
            var bookings = await _bookingService.GetBookingsAsync(queryParams, userId, role);
            return Ok(ApiResponse<PagedResult<BookingDTO>>.SuccessResponse(bookings, "Bookings retrieved successfully"));
        }

        /// <summary>
        /// Get available booking slots for a specific lab zone and date
        /// </summary>
        [HttpGet("available-slots")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<AvailableSlotDTO>>>> GetAvailableSlots([FromQuery] AvailableSlotQueryDTO query)
        {
            if (!ModelState.IsValid)
                throw new BadRequestException("Invalid slot query parameters");

            var (userId, role) = GetRequesterContext();
            var slots = await _bookingService.GetAvailableSlotsAsync(query, userId, role);
            return Ok(ApiResponse<IEnumerable<AvailableSlotDTO>>.SuccessResponse(slots, "Available slots retrieved successfully"));
        }

        /// <summary>
        /// Get booking by ID 
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Booking details</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<BookingDTO>>> GetBookingById(int id)
        {
            var (userId, role) = GetRequesterContext();
            var booking = await _bookingService.GetBookingByIdAsync(id, userId, role);
            if (booking == null)
            {
                throw new NotFoundException("Booking", id);
            }

            return Ok(ApiResponse<BookingDTO>.SuccessResponse(booking, "Booking retrieved successfully"));
        }

        /// <summary>
        /// Create a new booking
        /// </summary>
        /// <param name="createBookingDTO">Booking creation data</param>
        /// <returns>Created booking</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<BookingDTO>>> CreateBooking([FromBody] CreateBookingDTO createBookingDTO)
        {
            if (!ModelState.IsValid) 
                throw new BadRequestException("Invalid booking data");

            var (userId, role) = GetRequesterContext();

            if (role == Constant.UserRole.Member)
            {
                createBookingDTO.UserId = userId;
            }

            var booking = await _bookingService.CreateBookingAsync(createBookingDTO, userId, role);
            return CreatedAtAction(
                nameof(GetBookingById),
                new { id = booking.BookingId },
                ApiResponse<BookingDTO>.SuccessResponse(booking, "Booking created successfully")
            );
        }

        /// <summary>
        /// Update booking by ID
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="updateBookingDTO">Booking update data</param>
        /// <returns>Updated booking</returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<BookingDTO>>> UpdateBooking(int id, [FromBody] UpdateBookingDTO updateBookingDTO)
        {
            if (!ModelState.IsValid)
                throw new BadRequestException("Invalid booking data");

            var (userId, role) = GetRequesterContext();
            var booking = await _bookingService.UpdateBookingAsync(id, updateBookingDTO, userId, role);
            if (booking == null)
                throw new NotFoundException("Booking", id);

            return Ok(ApiResponse<BookingDTO>.SuccessResponse(booking, "Booking updated successfully"));
        }

        /// <summary>
        /// Delete booking by ID
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.Admin)},{nameof(Constant.UserRole.SchoolManager)}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteBooking(int id)
        {
            var (userId, role) = GetRequesterContext();
            var result = await _bookingService.DeleteBookingAsync(id, userId, role);
            if (!result)
                throw new NotFoundException("Booking", id);

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Booking deleted successfully"));
        }
    }
}
