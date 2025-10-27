using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Constants;
using LabManagement.Common.Exceptions;
using LabManagement.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabManagement.API.Controllers
{
    /// <summary>
    /// Notification management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Get all notifications (Admin, SchoolManager, LabManager)
        /// </summary>
        /// <returns>List of notifications</returns>
        [HttpGet]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<NotificationDTO>>>> GetAllNotifications()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Ok(ApiResponse<IEnumerable<NotificationDTO>>.SuccessResponse(
                notifications,
                "Notifications retrieved successfully"));
        }

        /// <summary>
        /// Get notification by ID
        /// </summary>
        /// <param name="id">Notification ID</param>
        /// <returns>Notification details</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}, {nameof(Constant.UserRole.SecurityLab)}")]
        public async Task<ActionResult<ApiResponse<NotificationDTO>>> GetNotificationById(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);

            if (notification == null)
                throw new NotFoundException("Notification", id);

            return Ok(ApiResponse<NotificationDTO>.SuccessResponse(
                notification,
                "Notification retrieved successfully"));
        }

        /// <summary>
        /// Create a new notification (Admin, SchoolManager, LabManager)
        /// </summary>
        /// <param name="createNotificationDto">Notification creation data</param>
        /// <returns>Created notification</returns>
        [HttpPost]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}, {nameof(Constant.UserRole.SecurityLab)}")]
        public async Task<ActionResult<ApiResponse<NotificationDTO>>> CreateNotification(
            [FromBody] CreateNotificationDTO createNotificationDto)
        {
            if (!ModelState.IsValid)
                throw new BadRequestException("Invalid notification data");

            var notification = await _notificationService.CreateNotificationAsync(createNotificationDto);

            return CreatedAtAction(
                nameof(GetNotificationById),
                new { id = notification.NotificationId },
                ApiResponse<NotificationDTO>.SuccessResponse(
                    notification,
                    "Notification created successfully"));
        }

        /// <summary>
        /// Update notification by ID (Admin, SchoolManager, LabManager)
        /// </summary>
        /// <param name="id">Notification ID</param>
        /// <param name="updateNotificationDto">Notification update data</param>
        /// <returns>Updated notification</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}, {nameof(Constant.UserRole.SecurityLab)}")]
        public async Task<ActionResult<ApiResponse<NotificationDTO>>> UpdateNotification(
            int id,
            [FromBody] UpdateNotificationDTO updateNotificationDto)
        {
            if (!ModelState.IsValid)
                throw new BadRequestException("Invalid notification data");

            // Check if notification exists
            if (!await _notificationService.NotificationIdExistsAsync(id))
                throw new NotFoundException("Notification", id);

            var notification = await _notificationService.UpdateNotificationAsync(id, updateNotificationDto);

            if (notification == null)
                throw new NotFoundException("Notification", id);

            return Ok(ApiResponse<NotificationDTO>.SuccessResponse(
                notification,
                "Notification updated successfully"));
        }

        /// <summary>
        /// Delete notification by ID (Admin only)
        /// </summary>
        /// <param name="id">Notification ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}, {nameof(Constant.UserRole.SecurityLab)}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteNotification(int id)
        {
            var result = await _notificationService.DeleteNotificationAsync(id);

            if (!result)
                throw new NotFoundException("Notification", id);

            return Ok(ApiResponse<object>.SuccessResponse(
                new { deletedNotificationId = id },
                "Notification deleted successfully"));
        }

        /// <summary>
        /// Mark notification as read
        /// </summary>
        /// <param name="id">Notification ID</param>
        /// <returns>Success message</returns>
        [HttpPatch("{id}/mark-as-read")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}, {nameof(Constant.UserRole.SecurityLab)}")]
        public async Task<ActionResult<ApiResponse<object>>> MarkAsRead(int id)
        {
            var result = await _notificationService.MarkAsReadAsync(id);

            if (!result)
                throw new NotFoundException("Notification", id);

            return Ok(ApiResponse<object>.SuccessResponse(
                new { notificationId = id, isRead = true },
                "Notification marked as read"));
        }

        /// <summary>
        /// Check if notification exists
        /// </summary>
        /// <param name="id">Notification ID</param>
        /// <returns>Boolean result</returns>
        [HttpGet("{id}/exists")]
        [Authorize(Roles = $"{nameof(Constant.UserRole.SchoolManager)},{nameof(Constant.UserRole.Admin)}, {nameof(Constant.UserRole.SecurityLab)}")]
        public async Task<ActionResult<ApiResponse<object>>> NotificationExists(int id)
        {
            var exists = await _notificationService.NotificationIdExistsAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(
                new { notificationId = id, exists },
                exists ? "Notification exists" : "Notification does not exist"));
        }
    }
}