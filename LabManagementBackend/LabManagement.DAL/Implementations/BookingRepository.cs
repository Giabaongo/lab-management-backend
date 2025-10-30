using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using System.Linq;

namespace LabManagement.DAL.Implementations
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(LabManagementDbContext context) : base(context)
        {
        }

        public IQueryable<Booking> GetBookingsQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }
}
