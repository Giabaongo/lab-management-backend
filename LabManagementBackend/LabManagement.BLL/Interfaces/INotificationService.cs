using LabManagement.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDTO>> GetAllNotificationsAsync();
        Task<NotificationDTO?> GetNotificationByIdAsync(int id);
        Task<NotificationDTO> CreateNotificationAsync(NotificationDTO notificationDTO);
        Task<NotificationDTO?> UpdateNotificationAsync(int id, NotificationDTO notificationDTO);
        Task<bool> DeleteNotificationAsync(int id);
        Task<bool> NotificationIdExistsAsync(int id);
        Task<bool> MarkAsReadAsync(int id);
    }
}
