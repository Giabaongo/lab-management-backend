using System.Threading;
using System.Threading.Tasks;

namespace LabManagement.DAL.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IUserRepository Users { get; }
        ILabRepository Labs { get; }
        IBookingRepository Bookings { get; }
        ILabZoneRepository LabZones { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        
    }
}
