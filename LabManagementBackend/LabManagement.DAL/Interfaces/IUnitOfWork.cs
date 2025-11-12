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
        IActivityTypeRepository ActivityTypes { get; }
        ILabEventRepository LabEvents { get; }
        ISecurityLogRepository SecurityLogs { get; }
        IEquipmentRepository Equipment { get; }
        INotificationRepository Notifications { get; }
        IReportRepository Reports { get; }
        IDepartmentRepository Departments { get; }
        IUserDepartmentRepository UserDepartments { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        
    }
}
