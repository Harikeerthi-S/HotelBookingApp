using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using HotelBookingAppWebApi.Interfaces;

namespace HotelBookingApp.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<int, Notification> _notificationRepository;
        private readonly IRepository<int, User> _userRepository;

        public NotificationService(
            IRepository<int, Notification> notificationRepository,
            IRepository<int, User> userRepository)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
        }

        // =====================================
        // CREATE NOTIFICATION
        // =====================================
        public async Task<NotificationResponseDto> CreateNotificationAsync(CreateNotificationDto dto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(dto.UserId);
                if (user == null) throw new Exception("User not found.");
                if (string.IsNullOrWhiteSpace(dto.Message)) throw new Exception("Notification message cannot be empty.");

                var notification = new Notification
                {
                    UserId = dto.UserId,
                    Message = dto.Message,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                var createdNotification = await _notificationRepository.AddAsync(notification);

                return new NotificationResponseDto
                {
                    NotificationId = createdNotification.NotificationId,
                    UserId = createdNotification.UserId,
                    Message = createdNotification.Message,
                    IsRead = createdNotification.IsRead,
                    CreatedAt = createdNotification.CreatedAt
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating notification: {ex.Message}");
            }
        }

        // =====================================
        // GET ALL NOTIFICATIONS FOR USER
        // =====================================
        public async Task<IEnumerable<NotificationResponseDto>> GetUserNotificationsAsync(int userId)
        {
            try
            {
                var allNotifications = await _notificationRepository.GetAllAsync();
                var userNotifications = allNotifications
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Select(n => new NotificationResponseDto
                    {
                        NotificationId = n.NotificationId,
                        UserId = n.UserId,
                        Message = n.Message,
                        IsRead = n.IsRead,
                        CreatedAt = n.CreatedAt
                    });

                return userNotifications;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving notifications: {ex.Message}");
            }
        }

        // =====================================
        // GET NOTIFICATION BY ID
        // =====================================
        public async Task<NotificationResponseDto?> GetNotificationByIdAsync(int notificationId)
        {
            try
            {
                var notification = await _notificationRepository.GetByIdAsync(notificationId);
                if (notification == null) return null;

                return new NotificationResponseDto
                {
                    NotificationId = notification.NotificationId,
                    UserId = notification.UserId,
                    Message = notification.Message,
                    IsRead = notification.IsRead,
                    CreatedAt = notification.CreatedAt
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving notification: {ex.Message}");
            }
        }

        // =====================================
        // MARK AS READ
        // =====================================
        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            try
            {
                var notification = await _notificationRepository.GetByIdAsync(notificationId);
                if (notification == null) return false;
                if (notification.IsRead) return true;

                notification.IsRead = true;
                await _notificationRepository.UpdateAsync(notification.NotificationId, notification);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error marking notification as read: {ex.Message}");
            }
        }

        // =====================================
        // DELETE NOTIFICATION
        // =====================================
        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            try
            {
                var deleted = await _notificationRepository.DeleteAsync(notificationId);
                return deleted != null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting notification: {ex.Message}");
            }
        }
    }
}