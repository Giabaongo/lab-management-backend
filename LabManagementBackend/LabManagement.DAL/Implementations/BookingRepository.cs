using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<List<Booking>> GetBookingsInRangeAsync(int labId, int zoneId, DateTime rangeStart, DateTime rangeEnd)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(b => b.LabId == labId
                            && b.ZoneId == zoneId
                            && b.StartTime < rangeEnd
                            && b.EndTime > rangeStart)
                .ToListAsync();
        }
    }
}
