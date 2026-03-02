using HotelBookingApp.Models.Dtos;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints require authentication
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // ==========================================
        // CREATE NOTIFICATION (Admin Only)
        // ==========================================
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateNotification(CreateNotificationDto dto)
        {
            try
            {
                var result = await _notificationService.CreateNotificationAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Failed to create notification.",
                    error = ex.Message
                });
            }
        }

        // ==========================================
        // GET MY NOTIFICATIONS (User)
        // ==========================================
        [HttpGet("my")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetMyNotifications()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var notifications = await _notificationService
                    .GetUserNotificationsAsync(userId);

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error retrieving notifications.",
                    error = ex.Message
                });
            }
        }

        // ==========================================
        // GET NOTIFICATION BY ID (Admin or Owner)
        // ==========================================
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            try
            {
                var notification = await _notificationService
                    .GetNotificationByIdAsync(id);

                if (notification == null)
                    return NotFound(new { message = "Notification not found." });

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                // If User role → allow only own notification
                if (userRole == "User" && notification.UserId != userId)
                    return Forbid();

                return Ok(notification);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error retrieving notification.",
                    error = ex.Message
                });
            }
        }

        // ==========================================
        // MARK AS READ (User only - Own Notification)
        // ==========================================
        [HttpPut("{id}/read")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var notification = await _notificationService
                    .GetNotificationByIdAsync(id);

                if (notification == null)
                    return NotFound(new { message = "Notification not found." });

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                if (notification.UserId != userId)
                    return Forbid();

                var result = await _notificationService.MarkAsReadAsync(id);

                return Ok(new { message = "Notification marked as read." });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Failed to update notification.",
                    error = ex.Message
                });
            }
        }

        // ==========================================
        // DELETE NOTIFICATION (Admin or Owner)
        // ==========================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            try
            {
                var notification = await _notificationService
                    .GetNotificationByIdAsync(id);

                if (notification == null)
                    return NotFound(new { message = "Notification not found." });

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                if (userRole == "User" && notification.UserId != userId)
                    return Forbid();

                var result = await _notificationService.DeleteNotificationAsync(id);

                return Ok(new { message = "Notification deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Failed to delete notification.",
                    error = ex.Message
                });
            }
        }
    }
}
