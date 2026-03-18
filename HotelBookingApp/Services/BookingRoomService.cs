using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using HotelBookingApp.Repositories;
using HotelBookingAppWebApi.Interfaces;
namespace HotelBookingApp.Services
{
    public class BookingRoomService : IBookingRoomService
    {
        private readonly IRepository<int, BookingRoom> _bookingRoomRepo;
        private readonly IRepository<int, Booking> _bookingRepo;
        private readonly IRepository<int, Room> _roomRepo;

        public BookingRoomService(
            IRepository<int, BookingRoom> bookingRoomRepo,
            IRepository<int, Booking> bookingRepo,
            IRepository<int, Room> roomRepo)
        {
            _bookingRoomRepo = bookingRoomRepo ?? throw new ArgumentNullException(nameof(bookingRoomRepo));
            _bookingRepo = bookingRepo ?? throw new ArgumentNullException(nameof(bookingRepo));
            _roomRepo = roomRepo ?? throw new ArgumentNullException(nameof(roomRepo));
        }

        public async Task<BookingRoomResponseDto> CreateBookingRoomAsync(CreateBookingRoomDto dto)
        {
            try
            {
                var booking = await _bookingRepo.GetByIdAsync(dto.BookingId);
                if (booking == null) throw new InvalidOperationException("Booking not found.");

                var room = await _roomRepo.GetByIdAsync(dto.RoomId);
                if (room == null || !room.IsAvailable)
                    throw new InvalidOperationException("Room not found or not available.");

                var entity = new BookingRoom
                {
                    BookingId = dto.BookingId,
                    RoomId = dto.RoomId,
                    PricePerNight = dto.PricePerNight,
                    NumberOfRooms = dto.NumberOfRooms
                };

                var created = await _bookingRoomRepo.AddAsync(entity);

                return new BookingRoomResponseDto
                {
                    BookingRoomId = created.BookingRoomId,
                    BookingId = created.BookingId,
                    RoomId = created.RoomId,
                    PricePerNight = created.PricePerNight,
                    NumberOfRooms = created.NumberOfRooms
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating booking room: {ex.Message}", ex);
            }
        }

        public async Task<BookingRoomResponseDto?> GetBookingRoomByIdAsync(int bookingRoomId)
        {
            try
            {
                var br = await _bookingRoomRepo.GetByIdAsync(bookingRoomId);
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
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving booking room with ID {bookingRoomId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<BookingRoomResponseDto>> GetBookingRoomsByBookingIdAsync(int bookingId)
        {
            try
            {
                var list = await _bookingRoomRepo.GetAllAsync();
                var filtered = list.Where(br => br.BookingId == bookingId);

                return filtered.Select(br => new BookingRoomResponseDto
                {
                    BookingRoomId = br.BookingRoomId,
                    BookingId = br.BookingId,
                    RoomId = br.RoomId,
                    PricePerNight = br.PricePerNight,
                    NumberOfRooms = br.NumberOfRooms
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving booking rooms for booking ID {bookingId}: {ex.Message}", ex);
            }
        }

        public async Task<BookingRoomResponseDto?> UpdateBookingRoomAsync(int bookingRoomId, CreateBookingRoomDto dto)
        {
            try
            {
                var br = await _bookingRoomRepo.GetByIdAsync(bookingRoomId);
                if (br == null) return null;

                var room = await _roomRepo.GetByIdAsync(dto.RoomId);
                if (room == null || !room.IsAvailable)
                    throw new InvalidOperationException("Room not found or not available.");

                br.RoomId = dto.RoomId;
                br.PricePerNight = dto.PricePerNight;
                br.NumberOfRooms = dto.NumberOfRooms;

                var updated = await _bookingRoomRepo.UpdateAsync(bookingRoomId, br);

                return new BookingRoomResponseDto
                {
                    BookingRoomId = updated.BookingRoomId,
                    BookingId = updated.BookingId,
                    RoomId = updated.RoomId,
                    PricePerNight = updated.PricePerNight,
                    NumberOfRooms = updated.NumberOfRooms
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating booking room with ID {bookingRoomId}: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteBookingRoomAsync(int bookingRoomId)
        {
            try
            {
                var br = await _bookingRoomRepo.GetByIdAsync(bookingRoomId);
                if (br == null) return false;

                await _bookingRoomRepo.DeleteAsync(bookingRoomId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting booking room with ID {bookingRoomId}: {ex.Message}", ex);
            }
        }
    }
}
