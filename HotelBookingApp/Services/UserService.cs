using FirstAPI.Interfaces;
using HotelBookingApp.DTOs.User;
using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using HotelBookingAppWebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBookingApp.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<int, User> _userRepository;
        private readonly IPasswordService _passwordService;

        public UserService(IRepository<int, User> userRepository, IPasswordService passwordService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
        }

        // Register a new user
        public async Task<RegisterUserResponseDTO> RegisterUser(RegisterUserRequestDTO request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                // Check if email already exists (in-memory because no predicate support)
                var users = await _userRepository.GetAllAsync();
                if (users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
                    throw new ApplicationException("User with this email already exists.");

                var passwordHash = _passwordService.HashPassword(request.Password);

                var user = new User
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    Phone = request.Phone,
                    Role = request.Role.ToLower(),
                    PasswordHash = passwordHash
                };

                var addedUser = await _userRepository.AddAsync(user);

                return new RegisterUserResponseDTO
                {
                    UserId = addedUser.UserId,
                    UserName = addedUser.UserName,
                    Email = addedUser.Email,
                    Role = addedUser.Role
                };
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                throw new ApplicationException($"Error registering user: {ex.Message}", ex);
            }
        }

        // Get a single user by ID
        public async Task<GetUsersResponseDTO> GetUserById(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new ApplicationException($"User with ID {userId} not found.");

                return new GetUsersResponseDTO
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving user: {ex.Message}", ex);
            }
        }

        // Get all users
        public async Task<IEnumerable<GetUsersResponseDTO>> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                return users.Select(u => new GetUsersResponseDTO
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving all users: {ex.Message}", ex);
            }
        }

        // Delete a user
        public async Task<bool> DeleteUser(int userId)
        {
            try
            {
                var deleted = await _userRepository.DeleteAsync(userId);
                return deleted != null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting user: {ex.Message}", ex);
            }
        }
    }
}