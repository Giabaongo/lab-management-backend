using LabManagement.BLL.DTOs;
using LabManagement.Common.Models;

namespace LabManagement.BLL.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDTO>> GetAllNotificationsAsync();
        Task<PagedResult<NotificationDTO>> GetNotificationsAsync(QueryParameters queryParams);
        Task<NotificationDTO?> GetNotificationByIdAsync(int id);
        Task<NotificationDTO> CreateNotificationAsync(CreateNotificationDTO createNotificationDTO);
        Task<NotificationDTO?> UpdateNotificationAsync(int id, UpdateNotificationDTO updateNotificationDTO);
        Task<bool> DeleteNotificationAsync(int id);
        Task<bool> NotificationIdExistsAsync(int id);
        Task<bool> MarkAsReadAsync(int id);
    }
}
