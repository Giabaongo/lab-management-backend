using LabManagement.DAL.Models;
using System.Linq;

namespace LabManagement.DAL.Interfaces
{
    public interface ILabRepository : IGenericRepository<Lab>
    {
        IQueryable<Lab> GetLabsQueryable();
    }
}
