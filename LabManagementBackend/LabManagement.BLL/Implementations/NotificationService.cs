using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        
        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<NotificationDTO> CreateNotificationAsync(CreateNotificationDTO createNotificationDTO)
        {
            var notification = _mapper.Map<Notification>(createNotificationDTO);
            notification.SentAt = DateTime.UtcNow; // Auto-set sent time
            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<NotificationDTO>(notification);
        }

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            // ✅ FIXED: Use Notifications instead of LabZones
            var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
            if(notification == null) return false;
            await _unitOfWork.Notifications.DeleteAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<NotificationDTO>> GetAllNotificationsAsync()
        {
            var notification = await _unitOfWork.Notifications.GetAllAsync();
            return _mapper.Map<IEnumerable<NotificationDTO>>(notification);
        }

        public async Task<NotificationDTO?> GetNotificationByIdAsync(int id)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
            return notification == null ? null : _mapper.Map<NotificationDTO?>(notification);
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
            if (notification == null) return false;

            // Only update if not already read
            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _unitOfWork.Notifications.UpdateAsync(notification);
                await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> NotificationIdExistsAsync(int id)
        {
            return await _unitOfWork.Notifications.ExistsAsync(n => n.NotificationId == id);
        }

        public async Task<NotificationDTO?> UpdateNotificationAsync(int id, UpdateNotificationDTO updateNotificationDTO)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
            if(notification == null) return null;
     
            // ✅ FIXED: Map DTO to entity correctly
            _mapper.Map(updateNotificationDTO, notification);
            await _unitOfWork.Notifications.UpdateAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<NotificationDTO>(notification);
        }
    }
}
