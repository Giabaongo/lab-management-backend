using LabManagement.DAL.Models;
using System.Linq;

namespace LabManagement.DAL.Interfaces
{
    public interface IEquipmentRepository : IGenericRepository<Equipment>
    {
        IQueryable<Equipment> GetEquipmentQueryable();
    }
}
