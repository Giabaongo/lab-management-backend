using LabManagement.DAL.Models;
using System.Linq;

namespace LabManagement.DAL.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        IQueryable<Notification> GetNotificationsQueryable();
    }
}
