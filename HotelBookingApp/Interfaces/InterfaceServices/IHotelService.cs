using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Interfaces.InterfaceServices
{
    public interface IHotelService
    {
        // ================= CREATE =================
        Task<HotelResponseDto> CreateHotelAsync(CreateHotelDto dto);

        // ================= READ =================
        Task<HotelResponseDto?> GetHotelByIdAsync(int hotelId);

        Task<PagedResponseDto<HotelResponseDto>> GetHotelsPagedAsync(PagedRequestDto request);

        // ================= FILTER =================
        Task<PagedResponseDto<HotelResponseDto>> FilterHotelsPagedAsync(
            HotelFilterDto filter,
            PagedRequestDto request);

        // ================= SEARCH =================
        Task<IEnumerable<HotelResponseDto>> SearchHotelsAsync(string location);

        // ================= UPDATE =================
        Task<HotelResponseDto?> UpdateHotelAsync(int hotelId, CreateHotelDto dto);

        // ================= SOFT DELETE =================
        Task<bool> DeactivateHotelAsync(int hotelId);
    }
}