using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using System.Linq;

namespace LabManagement.DAL.Implementations
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(LabManagementDbContext context) : base(context)
        {
        }

        public IQueryable<Notification> GetNotificationsQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }
}
