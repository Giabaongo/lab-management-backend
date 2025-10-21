using LabManagement.BLL.DTOs;

namespace LabManagement.BLL.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDTO>> GetAllBookingsAsync();
        Task<BookingDTO?> GetBookingByIdAsync(int id);
        Task<BookingDTO> CreateBookingAsync(CreateBookingDTO createBookingDTO);
        Task<BookingDTO?> UpdateBookingAsync(int id, UpdateBookingDTO updateBookingDTO);
        Task<bool> DeleteBookingAsync(int id);
        Task<bool> BookingExistsAsync(int id);
    }
}
