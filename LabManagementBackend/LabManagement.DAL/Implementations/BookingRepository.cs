using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;

namespace LabManagement.DAL.Implementations
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(LabManagementDbContext context) : base(context)
        {
        }
    }
}
