using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Interfaces.InterfaceServices
{
    public interface IBookingService
    {
        // Create a new booking
        Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto);

        // Confirm a booking
        Task<BookingResponseDto> ConfirmBookingAsync(int bookingId);

        // Cancel a booking
        Task<bool> CancelBookingAsync(int bookingId);

        // Complete a booking
        Task<BookingResponseDto> CompleteBookingAsync(int bookingId);

        // Get a single booking by its ID
        Task<BookingResponseDto?> GetBookingByIdAsync(int bookingId);

        // Get bookings by a user with pagination
        Task<PagedResponseDto<BookingResponseDto>> GetBookingsByUserAsync(int userId, PagedRequestDto request);

        // Get bookings by hotel with pagination
        Task<PagedResponseDto<BookingResponseDto>> GetBookingsByHotelAsync(int hotelId, PagedRequestDto request);

        // Get all pending bookings for a hotel (no pagination)
        Task<List<BookingResponseDto>> GetPendingBookingsForHotelAsync(int hotelId);
    }
}