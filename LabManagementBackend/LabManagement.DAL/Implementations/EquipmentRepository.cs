using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.DAL.Implementations
{
    public class EquipmentRepository : GenericRepository<Equipment>, IEquipmentRepository
    {
        public EquipmentRepository(LabManagementDbContext context) : base(context)
        {
        }
}
}

