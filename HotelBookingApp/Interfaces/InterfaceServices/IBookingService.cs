using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Services
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto);
        Task<BookingResponseDto?> GetBookingByIdAsync(int bookingId);
        Task<PagedResponseDto<BookingResponseDto>> GetBookingsByUserAsync(int userId, PagedRequestDto pageRequest);
        Task<PagedResponseDto<BookingResponseDto>> GetBookingsByHotelAsync(int hotelId, PagedRequestDto pageRequest);
        Task<bool> CancelBookingAsync(int bookingId);
        Task<BookingResponseDto> CompleteBookingAsync(int bookingId);
    }
}