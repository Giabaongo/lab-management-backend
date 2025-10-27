using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;

namespace LabManagement.DAL.Implementations
{
    public class LabEventRepository : GenericRepository<LabEvent>, ILabEventRepository
    {
        public LabEventRepository(LabManagementDbContext context) : base(context)
        {
        }
    }
}
