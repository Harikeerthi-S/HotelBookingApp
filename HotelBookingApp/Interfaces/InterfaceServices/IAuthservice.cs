using HotelBookingApp.DTOs.User;

namespace HotelBookingApp.Interfaces.InterfaceServices
{
    public interface IAuthService
    {
        Task<LoginUserResponseDTO> LoginUser(LoginUserRequestDTO request);
    }
}