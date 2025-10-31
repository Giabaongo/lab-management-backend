using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.DAL.Implementations
{
    public class LabZoneRepository : GenericRepository<LabZone>, ILabZoneRepository
    {
        //private readonly LabManagementDbContext _context;
        public LabZoneRepository(LabManagementDbContext context) : base(context)
        {
        }

        //Nếu muốn custom
        //public new async Task<IEnumerable<LabZone>> GetAllAsync()
        //{
        //    return await _dbSet.ToListAsync();
        //}

    }
}
