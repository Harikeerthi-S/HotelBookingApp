using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Services
{
    public class BookingService : IBookingService
    {
        private readonly HotelBookingContext _context;

        public BookingService(HotelBookingContext context)
        {
            _context = context;
        }

        // Create a new booking
        public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto)
        {
            // Ensure at least 1-night stay
            if (dto.CheckOut <= dto.CheckIn)
                throw new ArgumentException("Check-out date must be after check-in date.");

            // Check if hotel exists and is active
            var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.HotelId == dto.HotelId && h.IsActive);
            if (hotel == null) throw new InvalidOperationException("Hotel does not exist or is inactive.");

            // Check if room exists and is available
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == dto.RoomId && r.IsAvailable);
            if (room == null) throw new InvalidOperationException("Room does not exist or is not available.");

            // Check for overlapping bookings
            var overlappingBooking = await _context.Bookings
                .AnyAsync(b => b.RoomId == dto.RoomId &&
                               ((dto.CheckIn >= b.CheckIn && dto.CheckIn < b.CheckOut) ||
                                (dto.CheckOut > b.CheckIn && dto.CheckOut <= b.CheckOut) ||
                                (dto.CheckIn <= b.CheckIn && dto.CheckOut >= b.CheckOut)));

            if (overlappingBooking)
                throw new InvalidOperationException("Room is already booked for the selected dates.");

            // Calculate total amount
            var totalDays = (decimal)(dto.CheckOut - dto.CheckIn).TotalDays;
            var totalAmount = totalDays * room.PricePerNight;

            // Create booking
            var booking = new Booking
            {
                UserId = dto.UserId,
                HotelId = dto.HotelId,
                RoomId = dto.RoomId,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                TotalAmount = totalAmount,
                Status = "Confirmed"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return new BookingResponseDto
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                HotelId = booking.HotelId,
                RoomId = booking.RoomId,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status
            };
        }

        // Get booking by ID
        public async Task<BookingResponseDto?> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _context.Bookings.AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null) return null;

            return new BookingResponseDto
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                HotelId = booking.HotelId,
                RoomId = booking.RoomId,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status
            };
        }

        // Get bookings for a user with pagination
        public async Task<PagedResponseDto<BookingResponseDto>> GetBookingsByUserAsync(int userId, PagedRequestDto pageRequest)
        {
            if (pageRequest.PageNumber <= 0) pageRequest.PageNumber = 1;
            if (pageRequest.PageSize <= 0) pageRequest.PageSize = 10;

            var query = _context.Bookings.AsNoTracking().Where(b => b.UserId == userId);
            var totalRecords = await query.CountAsync();

            var bookings = await query.OrderByDescending(b => b.CheckIn)
                .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize)
                .Select(b => new BookingResponseDto
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    HotelId = b.HotelId,
                    RoomId = b.RoomId,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    TotalAmount = b.TotalAmount,
                    Status = b.Status
                })
                .ToListAsync();

            return new PagedResponseDto<BookingResponseDto>
            {
                Data = bookings,
                PageNumber = pageRequest.PageNumber,
                PageSize = pageRequest.PageSize,
                TotalRecords = totalRecords
            };
        }

        // Get bookings for a hotel with pagination
        public async Task<PagedResponseDto<BookingResponseDto>> GetBookingsByHotelAsync(int hotelId, PagedRequestDto pageRequest)
        {
            if (pageRequest.PageNumber <= 0) pageRequest.PageNumber = 1;
            if (pageRequest.PageSize <= 0) pageRequest.PageSize = 10;

            var query = _context.Bookings.AsNoTracking().Where(b => b.HotelId == hotelId);
            var totalRecords = await query.CountAsync();

            var bookings = await query.OrderByDescending(b => b.CheckIn)
                .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize)
                .Select(b => new BookingResponseDto
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    HotelId = b.HotelId,
                    RoomId = b.RoomId,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    TotalAmount = b.TotalAmount,
                    Status = b.Status
                })
                .ToListAsync();

            return new PagedResponseDto<BookingResponseDto>
            {
                Data = bookings,
                PageNumber = pageRequest.PageNumber,
                PageSize = pageRequest.PageSize,
                TotalRecords = totalRecords
            };
        }

        // Cancel a booking
        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null) return false;

            if (booking.Status != "Confirmed")
                throw new InvalidOperationException($"Booking cannot be cancelled because its status is '{booking.Status}'.");

            booking.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return true;
        }

        // Complete a booking
        public async Task<BookingResponseDto> CompleteBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null) throw new InvalidOperationException("Booking not found.");

            if (booking.Status != "Confirmed")
                throw new InvalidOperationException($"Booking cannot be completed because its status is '{booking.Status}'.");

            booking.Status = "Completed";
            await _context.SaveChangesAsync();

            return new BookingResponseDto
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                HotelId = booking.HotelId,
                RoomId = booking.RoomId,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status
            };
        }
    }
}