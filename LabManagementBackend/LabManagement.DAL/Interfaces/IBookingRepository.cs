using LabManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabManagement.DAL.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        IQueryable<Booking> GetBookingsQueryable();
        Task<List<Booking>> GetBookingsInRangeAsync(int labId, int zoneId, DateTime rangeStart, DateTime rangeEnd);
    }
}
