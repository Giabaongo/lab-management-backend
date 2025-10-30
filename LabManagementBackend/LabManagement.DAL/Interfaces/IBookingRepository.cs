using LabManagement.DAL.Models;
using System.Linq;

namespace LabManagement.DAL.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        IQueryable<Booking> GetBookingsQueryable();
    }
}
