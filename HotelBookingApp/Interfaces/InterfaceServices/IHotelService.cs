using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Interfaces.InterfaceServices
{
    public interface IHotelService
    {
        // Create a new hotel
        Task<HotelResponseDto> CreateHotelAsync(CreateHotelDto dto);

        // Get hotel by Id
        Task<HotelResponseDto?> GetHotelByIdAsync(int hotelId);

        // Get paginated hotels (POST)
        Task<PagedResponseDto<HotelResponseDto>> GetHotelsPagedAsync(PagedRequestDto request);

        // Search hotels by location
        Task<IEnumerable<HotelResponseDto>> SearchHotelsAsync(string location);

        // Filter hotels with pagination
        Task<PagedResponseDto<HotelResponseDto>> FilterHotelsPagedAsync(HotelFilterDto filter, PagedRequestDto request);

        // Update hotel
        Task<HotelResponseDto?> UpdateHotelAsync(int hotelId, CreateHotelDto dto);

        // Deactivate hotel (soft delete)
        Task<bool> DeactivateHotelAsync(int hotelId);
    }
}