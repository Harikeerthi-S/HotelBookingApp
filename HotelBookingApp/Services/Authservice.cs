using FirstAPI.Interfaces;
using HotelBookingApp.DTOs.User;
using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models;
using HotelBookingAppWebApi.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBookingApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<int, User> _userRepository;
        private readonly IPasswordService _passwordService;

        public AuthService(IRepository<int, User> userRepository, IPasswordService passwordService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
        }

        public async Task<LoginUserResponseDTO> LoginUser(LoginUserRequestDTO request)
        {
            try
            {
                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                // Because repository doesn't support query filtering, fetch all and filter in-memory
                var users = await _userRepository.GetAllAsync();
                var user = users.FirstOrDefault(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

                if (user == null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
                    throw new ApplicationException("Invalid email or password.");

                return new LoginUserResponseDTO
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error logging in: {ex.Message}", ex);
            }
        }
    }
}