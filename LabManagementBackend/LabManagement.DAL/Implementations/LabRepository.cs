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
    public class LabRepository : GenericRepository<Lab>, ILabRepository
    {
        public LabRepository(LabManagementDbContext context) : base(context)
        {
        }
    }
}
