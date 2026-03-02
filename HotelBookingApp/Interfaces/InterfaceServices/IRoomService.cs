using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Services
{
    public interface IRoomService
    {
        Task<RoomResponseDto> CreateRoomAsync(CreateRoomDto dto);
        Task<RoomResponseDto?> GetRoomByIdAsync(int roomId);
        Task<IEnumerable<RoomResponseDto>> GetAllRoomsAsync(int? hotelId = null);
        Task<RoomResponseDto?> UpdateRoomAsync(int roomId, CreateRoomDto dto);
        Task<bool> DeactivateRoomAsync(int roomId);
        Task<IEnumerable<RoomResponseDto>> GetRoomsFilteredAsync(RoomFilterDto filter);

        // ✅ New paged method
        Task<PagedResponseDto<RoomResponseDto>> GetRoomsFilteredPagedAsync(RoomFilterDto filter, PagedRequestDto pageRequest);
    }
}

