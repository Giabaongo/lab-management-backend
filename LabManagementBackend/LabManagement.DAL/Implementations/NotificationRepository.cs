using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.DAL.Implementations
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(LabManagementDbContext context) : base(context)
        {
        }
    }
}
