using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;

namespace LabManagement.BLL.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BookingDTO> CreateBookingAsync(CreateBookingDTO createBookingDTO)
        {
            var booking = _mapper.Map<Booking>(createBookingDTO);
            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null) return false;

            await _unitOfWork.Bookings.DeleteAsync(booking);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BookingDTO>> GetAllBookingsAsync()
        {
            var bookings = await _unitOfWork.Bookings.GetAllAsync();
            return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
        }

        public async Task<BookingDTO?> GetBookingByIdAsync(int id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            return booking == null ? null : _mapper.Map<BookingDTO>(booking);
        }

        public async Task<bool> BookingExistsAsync(int id)
        {
            return await _unitOfWork.Bookings.ExistsAsync(b => b.BookingId == id);
        }

        public async Task<BookingDTO?> UpdateBookingAsync(int id, UpdateBookingDTO updateBookingDTO)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null) return null;

            _mapper.Map(updateBookingDTO, booking);
            await _unitOfWork.Bookings.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();
            
            return _mapper.Map<BookingDTO>(booking);
        }
    }
}
