using HotelBookingApp.Models;

namespace HotelBookingApp.DTOs.User
{
    public class RegisterUserResponseDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}