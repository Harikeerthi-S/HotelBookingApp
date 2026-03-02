using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Services
{
    public interface INotificationService
    {
        Task<NotificationResponseDto> CreateNotificationAsync(CreateNotificationDto dto);
        Task<IEnumerable<NotificationResponseDto>> GetUserNotificationsAsync(int userId);
        Task<NotificationResponseDto?> GetNotificationByIdAsync(int notificationId);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> DeleteNotificationAsync(int notificationId);
    }
}