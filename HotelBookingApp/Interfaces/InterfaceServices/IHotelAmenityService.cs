using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Interfaces.InterfaceServices
{
    public interface IHotelAmenityService
    {
        Task<IEnumerable<HotelAmenityResponseDto>> GetAllAsync();
        Task<HotelAmenityResponseDto?> GetByIdAsync(int id);
        Task<HotelAmenityResponseDto> CreateAsync(HotelAmenityDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
