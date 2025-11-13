using LabManagement.BLL.DTOs;
using LabManagement.Common.Constants;
using LabManagement.Common.Models;

namespace LabManagement.BLL.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDTO>> GetAllBookingsAsync();
        Task<PagedResult<BookingDTO>> GetBookingsAsync(QueryParameters queryParams);
        Task<BookingDTO?> GetBookingByIdAsync(int id);
        Task<BookingDTO> CreateBookingAsync(CreateBookingDTO createBookingDTO, int requesterId, Constant.UserRole requesterRole);
        Task<BookingDTO?> UpdateBookingAsync(int id, UpdateBookingDTO updateBookingDTO);
        Task<bool> DeleteBookingAsync(int id);
        Task<bool> BookingExistsAsync(int id);
        Task<IEnumerable<AvailableSlotDTO>> GetAvailableSlotsAsync(AvailableSlotQueryDTO query, int requesterId, Constant.UserRole requesterRole);
    }
}
