using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Interfaces.InterfaceServices
{
    public interface IAmenityService
    {
        Task<IEnumerable<AmenityResponseDto>> GetAllAsync();
        Task<AmenityResponseDto?> GetByIdAsync(int id);
        Task<AmenityResponseDto> CreateAsync(CreateAmenityDto dto);
        Task<bool> UpdateAsync(int id, CreateAmenityDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
