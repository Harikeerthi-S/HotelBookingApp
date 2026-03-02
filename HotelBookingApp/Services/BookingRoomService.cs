using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Services
{
    public class BookingRoomService : IBookingRoomService
    {
        private readonly HotelBookingContext _context;

        public BookingRoomService(HotelBookingContext context)
        {
            _context = context;
        }

        public async Task<BookingRoomResponseDto> CreateBookingRoomAsync(CreateBookingRoomDto dto)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == dto.BookingId);
            if (booking == null)
                throw new InvalidOperationException("Booking not found.");

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == dto.RoomId);
            if (room == null)
                throw new InvalidOperationException("Room not found.");

            if (!room.IsAvailable)
                throw new InvalidOperationException("Room is not available for booking.");

            var bookingRoom = new BookingRoom
            {
                BookingId = dto.BookingId,
                RoomId = dto.RoomId,
                PricePerNight = dto.PricePerNight,
                NumberOfRooms = dto.NumberOfRooms
            };

            _context.BookingRooms.Add(bookingRoom);
            await _context.SaveChangesAsync();

            return new BookingRoomResponseDto
            {
                BookingRoomId = bookingRoom.BookingRoomId,
                BookingId = bookingRoom.BookingId,
                RoomId = bookingRoom.RoomId,
                PricePerNight = bookingRoom.PricePerNight,
                NumberOfRooms = bookingRoom.NumberOfRooms
            };
        }

        public async Task<BookingRoomResponseDto?> GetBookingRoomByIdAsync(int bookingRoomId)
        {
            var br = await _context.BookingRooms.AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookingRoomId == bookingRoomId);

            if (br == null) return null;

            return new BookingRoomResponseDto
            {
                BookingRoomId = br.BookingRoomId,
                BookingId = br.BookingId,
                RoomId = br.RoomId,
                PricePerNight = br.PricePerNight,
                NumberOfRooms = br.NumberOfRooms
            };
        }

        public async Task<IEnumerable<BookingRoomResponseDto>> GetBookingRoomsByBookingIdAsync(int bookingId)
        {
            var list = await _context.BookingRooms
                .Where(b => b.BookingId == bookingId)
                .AsNoTracking()
                .ToListAsync();

            return list.Select(br => new BookingRoomResponseDto
            {
                BookingRoomId = br.BookingRoomId,
                BookingId = br.BookingId,
                RoomId = br.RoomId,
                PricePerNight = br.PricePerNight,
                NumberOfRooms = br.NumberOfRooms
            });
        }

        public async Task<BookingRoomResponseDto?> UpdateBookingRoomAsync(int bookingRoomId, CreateBookingRoomDto dto)
        {
            var br = await _context.BookingRooms.FirstOrDefaultAsync(b => b.BookingRoomId == bookingRoomId);
            if (br == null) return null;

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == dto.RoomId);
            if (room == null)
                throw new InvalidOperationException("Room not found.");

            if (!room.IsAvailable)
                throw new InvalidOperationException("Room is not available.");

            br.RoomId = dto.RoomId;
            br.PricePerNight = dto.PricePerNight;
            br.NumberOfRooms = dto.NumberOfRooms;

            await _context.SaveChangesAsync();

            return new BookingRoomResponseDto
            {
                BookingRoomId = br.BookingRoomId,
                BookingId = br.BookingId,
                RoomId = br.RoomId,
                PricePerNight = br.PricePerNight,
                NumberOfRooms = br.NumberOfRooms
            };
        }

        public async Task<bool> DeleteBookingRoomAsync(int bookingRoomId)
        {
            var br = await _context.BookingRooms.FirstOrDefaultAsync(b => b.BookingRoomId == bookingRoomId);
            if (br == null) return false;

            _context.BookingRooms.Remove(br);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}