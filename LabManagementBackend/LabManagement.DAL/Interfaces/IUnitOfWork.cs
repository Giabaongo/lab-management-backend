using System.Threading;
using System.Threading.Tasks;

namespace LabManagement.DAL.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IUserRepository Users { get; }
        ILabRepository Labs { get; }
        IBookingRepository Bookings { get; }
        IActivityTypeRepository ActivityTypes { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        
    }
}
