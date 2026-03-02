using FirstAPI.Interfaces;
using HotelBookingApp.DTOs.User;
using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Services
{
    public class UserService : IUserService
    {
        private readonly HotelBookingContext _context;
        private readonly IPasswordService _passwordService;

        public UserService(HotelBookingContext context, IPasswordService passwordService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
        }

        // 🔹 Register a new user
        public async Task<RegisterUserResponseDTO> RegisterUser(RegisterUserRequestDTO request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingUser != null)
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

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return new RegisterUserResponseDTO
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                throw new ApplicationException($"Error registering user: {ex.Message}", ex);
            }
        }

        // 🔹 Login user
        public async Task<LoginUserResponseDTO> LoginUser(LoginUserRequestDTO request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
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

        // 🔹 Get a single user by ID
        public async Task<GetUsersResponseDTO> GetUserById(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
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

        // 🔹 Get all users
        public async Task<IEnumerable<GetUsersResponseDTO>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
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

        // 🔹 Delete a user
        public async Task<bool> DeleteUser(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting user: {ex.Message}", ex);
            }
        }
    }
}