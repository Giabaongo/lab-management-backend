using System.Threading;
using System.Threading.Tasks;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;

namespace LabManagement.DAL.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LabManagementDbContext _context;
        private IUserRepository? _userRepository;
        private ILabRepository? _labRepository;
        private IBookingRepository? _bookingRepository;
        private ILabZoneRepository? _labZoneRepository;
        private IActivityTypeRepository? _activityTypeRepository;
        private ILabEventRepository? _labEventRepository;
        private ISecurityLogRepository? _securityLogRepository;
        private IEquipmentRepository? _equipmentRepository;
        private INotificationRepository? _notificationRepository;

        public UnitOfWork(LabManagementDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _userRepository ??= new UserRepository(_context);

        public ILabRepository Labs => _labRepository ??= new LabRepository(_context);

        public IBookingRepository Bookings => _bookingRepository ??= new BookingRepository(_context);

        public ILabZoneRepository LabZones => _labZoneRepository ??= new LabZoneRepository(_context);
        public IActivityTypeRepository ActivityTypes => _activityTypeRepository ??= new ActivityTypeRepository(_context);

        public ILabEventRepository LabEvents => _labEventRepository ??= new LabEventRepository(_context);

        public ISecurityLogRepository SecurityLogs => _securityLogRepository ??= new SecurityLogRepository(_context);

        public IEquipmentRepository Equipment => _equipmentRepository ??= new EquipmentRepository(_context);

        public INotificationRepository Notifications => _notificationRepository ??= new NotificationRepository(_context);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}