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
    /// Booking management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Get all bookings
        /// </summary>
        /// <returns>List of bookings</returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<BookingDTO>>>> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(ApiResponse<IEnumerable<BookingDTO>>.SuccessResponse(bookings, "Bookings retrieved successfully"));
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
            var booking = await _bookingService.GetBookingByIdAsync(id);
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

            var booking = await _bookingService.CreateBookingAsync(createBookingDTO);
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

            // Check if booking exists
            if (!await _bookingService.BookingExistsAsync(id))
                throw new NotFoundException("Booking", id);

            var booking = await _bookingService.UpdateBookingAsync(id, updateBookingDTO);
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
            var result = await _bookingService.DeleteBookingAsync(id);
            if (!result)
                throw new NotFoundException("Booking", id);

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Booking deleted successfully"));
        }
    }
}
