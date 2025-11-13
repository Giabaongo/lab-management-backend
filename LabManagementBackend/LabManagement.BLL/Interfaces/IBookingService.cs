using LabManagement.BLL.DTOs;
using LabManagement.Common.Constants;
using LabManagement.Common.Models;

namespace LabManagement.BLL.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDTO>> GetAllBookingsAsync(int requesterId, Constant.UserRole requesterRole);
        Task<PagedResult<BookingDTO>> GetBookingsAsync(QueryParameters queryParams, int requesterId, Constant.UserRole requesterRole);
        Task<BookingDTO?> GetBookingByIdAsync(int id, int requesterId, Constant.UserRole requesterRole);
        Task<BookingDTO> CreateBookingAsync(CreateBookingDTO createBookingDTO, int requesterId, Constant.UserRole requesterRole);
        Task<BookingDTO?> UpdateBookingAsync(int id, UpdateBookingDTO updateBookingDTO, int requesterId, Constant.UserRole requesterRole);
        Task<bool> DeleteBookingAsync(int id, int requesterId, Constant.UserRole requesterRole);
        Task<IEnumerable<AvailableSlotDTO>> GetAvailableSlotsAsync(AvailableSlotQueryDTO query, int requesterId, Constant.UserRole requesterRole);
    }
}
