using LabManagement.DAL.Models;
using System.Linq;

namespace LabManagement.DAL.Interfaces
{
    public interface ILabEventRepository : IGenericRepository<LabEvent>
    {
        IQueryable<LabEvent> GetLabEventsQueryable();
    }
}
