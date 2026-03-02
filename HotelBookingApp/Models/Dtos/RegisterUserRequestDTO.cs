using HotelBookingApp.Models;
using System.Data;

namespace HotelBookingApp.DTOs.User
{
    public class RegisterUserRequestDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }

        public string Role { get; set; } = string.Empty;
    }
}