using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using System.Linq;

namespace LabManagement.DAL.Implementations
{
    public class EquipmentRepository : GenericRepository<Equipment>, IEquipmentRepository
    {
        public EquipmentRepository(LabManagementDbContext context) : base(context)
        {
        }
        
        public IQueryable<Equipment> GetEquipmentQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }
}
