using HotelBookingApp.DTOs.User;
using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Interfaces.InterfaceServices
{
    public interface IUserService
    {
        // 🔹 Register a new user
        Task<RegisterUserResponseDTO> RegisterUser(RegisterUserRequestDTO request);

        // 🔹 Login a user
        Task<LoginUserResponseDTO> LoginUser(LoginUserRequestDTO request);

        // 🔹 Get a user by ID
        Task<GetUsersResponseDTO> GetUserById(int userId);

        // 🔹 Get all users
        Task<IEnumerable<GetUsersResponseDTO>> GetAllUsers();

        // 🔹 Delete a user by ID
        Task<bool> DeleteUser(int userId);
    }
}