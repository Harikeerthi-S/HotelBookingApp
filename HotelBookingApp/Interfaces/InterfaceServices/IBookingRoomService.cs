using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Services
{
    public interface IBookingRoomService
    {
        Task<BookingRoomResponseDto> CreateBookingRoomAsync(CreateBookingRoomDto dto);
        Task<BookingRoomResponseDto?> GetBookingRoomByIdAsync(int bookingRoomId);
        Task<IEnumerable<BookingRoomResponseDto>> GetBookingRoomsByBookingIdAsync(int bookingId);
        Task<BookingRoomResponseDto?> UpdateBookingRoomAsync(int bookingRoomId, CreateBookingRoomDto dto);
        Task<bool> DeleteBookingRoomAsync(int bookingRoomId);
    }
}