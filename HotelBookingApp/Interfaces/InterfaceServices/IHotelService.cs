using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Services
{
    public interface IHotelService
    {
        Task<HotelResponseDto> CreateHotelAsync(CreateHotelDto dto);
        Task<HotelResponseDto?> GetHotelByIdAsync(int hotelId);
        Task<PagedResponseDto<HotelResponseDto>> GetHotelsPagedAsync(PagedRequestDto request);
        Task<IEnumerable<HotelResponseDto>> SearchHotelsAsync(string location);
        Task<PagedResponseDto<HotelResponseDto>> FilterHotelsPagedAsync(HotelFilterDto filter, PagedRequestDto request);
        Task<HotelResponseDto?> UpdateHotelAsync(int hotelId, CreateHotelDto dto);
        Task<bool> DeactivateHotelAsync(int hotelId);
    }
}