using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HotelBookingContext _context;

        public NotificationService(HotelBookingContext context)
        {
            _context = context;
        }

        // =====================================
        // CREATE NOTIFICATION
        // =====================================
        public async Task<NotificationResponseDto> CreateNotificationAsync(CreateNotificationDto dto)
        {
            try
            {
                // Validate user exists
                var userExists = await _context.Users
                    .AnyAsync(u => u.UserId == dto.UserId);

                if (!userExists)
                    throw new Exception("User not found.");

                if (string.IsNullOrWhiteSpace(dto.Message))
                    throw new Exception("Notification message cannot be empty.");

                var notification = new Notification
                {
                    UserId = dto.UserId,
                    Message = dto.Message,
                    IsRead = false, // default unread
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

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
                return await _context.Notifications
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Select(n => new NotificationResponseDto
                    {
                        NotificationId = n.NotificationId,
                        UserId = n.UserId,
                        Message = n.Message,
                        IsRead = n.IsRead,
                        CreatedAt = n.CreatedAt
                    })
                    .ToListAsync();
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
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

                if (notification == null)
                    return null;

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
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

                if (notification == null)
                    return false;

                if (notification.IsRead)
                    return true; // Already read

                notification.IsRead = true;

                await _context.SaveChangesAsync();
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
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

                if (notification == null)
                    return false;

                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting notification: {ex.Message}");
            }
        }
    }
}