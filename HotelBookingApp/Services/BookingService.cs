using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using HotelBookingAppWebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBookingApp.Services
{
    public class BookingService : IBookingService
    {
        private readonly IRepository<int, Booking> _bookingRepository;
        private readonly IRepository<int, Hotel> _hotelRepository;
        private readonly IRepository<int, Room> _roomRepository;

        public BookingService(
            IRepository<int, Booking> bookingRepository,
            IRepository<int, Hotel> hotelRepository,
            IRepository<int, Room> roomRepository)
        {
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
            _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
            _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
        }

        // CREATE BOOKING
        public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                var hotel = await _hotelRepository.GetByIdAsync(dto.HotelId);
                if (hotel == null || !hotel.IsActive) throw new InvalidOperationException("Hotel not found.");

                var room = await _roomRepository.GetByIdAsync(dto.RoomId);
                if (room == null) throw new InvalidOperationException("Room not found.");

                if (dto.CheckOut <= dto.CheckIn) throw new InvalidOperationException("Check-out must be after check-in.");

                var totalAmount = (decimal)(dto.CheckOut - dto.CheckIn).TotalDays * room.PricePerNight * dto.NumberOfRooms;

                var booking = new Booking
                {
                    UserId = dto.UserId,
                    HotelId = dto.HotelId,
                    RoomId = dto.RoomId,
                    CheckIn = dto.CheckIn,
                    CheckOut = dto.CheckOut,
                    TotalAmount = totalAmount,
                    Status = "Pending"
                };

                await _bookingRepository.AddAsync(booking);

                return new BookingResponseDto
                {
                    BookingId = booking.BookingId,
                    UserId = booking.UserId,
                    HotelId = booking.HotelId,
                    HotelName = hotel.HotelName,
                    RoomId = booking.RoomId,
                    NumberOfRooms = dto.NumberOfRooms,
                    CheckIn = booking.CheckIn,
                    CheckOut = booking.CheckOut,
                    TotalAmount = booking.TotalAmount,
                    Status = booking.Status
                };
            }
            catch (Exception ex)
            {
                // Replace Console.WriteLine with your logging mechanism
                Console.WriteLine($"Error in CreateBookingAsync: {ex.Message}");
                throw;
            }
        }

        // CONFIRM BOOKING
        public async Task<BookingResponseDto> ConfirmBookingAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(bookingId);
                if (booking == null) throw new InvalidOperationException("Booking not found.");

                booking.Status = "Confirmed";
                await _bookingRepository.UpdateAsync(bookingId, booking);

                var hotel = await _hotelRepository.GetByIdAsync(booking.HotelId);
                return MapToBookingDto(booking, 1, hotel?.HotelName ?? string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ConfirmBookingAsync: {ex.Message}");
                throw;
            }
        }

        // COMPLETE BOOKING
        public async Task<BookingResponseDto> CompleteBookingAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(bookingId);
                if (booking == null) throw new InvalidOperationException("Booking not found.");

                booking.Status = "Completed";
                await _bookingRepository.UpdateAsync(bookingId, booking);

                var hotel = await _hotelRepository.GetByIdAsync(booking.HotelId);
                return MapToBookingDto(booking, 1, hotel?.HotelName ?? string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CompleteBookingAsync: {ex.Message}");
                throw;
            }
        }

        // CANCEL BOOKING
        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(bookingId);
                if (booking == null) return false;

                booking.Status = "Cancelled";
                await _bookingRepository.UpdateAsync(bookingId, booking);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CancelBookingAsync: {ex.Message}");
                return false;
            }
        }

        // GET BOOKING BY ID
        public async Task<BookingResponseDto?> GetBookingByIdAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(bookingId);
                if (booking == null) return null;

                var hotel = await _hotelRepository.GetByIdAsync(booking.HotelId);
                return MapToBookingDto(booking, 1, hotel?.HotelName ?? string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingByIdAsync: {ex.Message}");
                return null;
            }
        }

        // GET BOOKINGS BY USER WITH PAGINATION
        public async Task<PagedResponseDto<BookingResponseDto>> GetBookingsByUserAsync(int userId, PagedRequestDto request)
        {
            try
            {
                if (request.PageNumber <= 0) request.PageNumber = 1;
                if (request.PageSize <= 0) request.PageSize = 10;

                var allBookings = await _bookingRepository.GetAllAsync();
                var filtered = allBookings.Where(b => b.UserId == userId).ToList();
                var total = filtered.Count;

                var paged = filtered
                    .OrderByDescending(b => b.CheckIn)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var data = new List<BookingResponseDto>();

                foreach (var booking in paged)
                {
                    var hotel = await _hotelRepository.GetByIdAsync(booking.HotelId);
                    data.Add(MapToBookingDto(booking, 1, hotel?.HotelName ?? string.Empty));
                }

                return new PagedResponseDto<BookingResponseDto>
                {
                    Data = data,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalRecords = total
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingsByUserAsync: {ex.Message}");
                throw;
            }
        }

        // GET BOOKINGS BY HOTEL WITH PAGINATION
        public async Task<PagedResponseDto<BookingResponseDto>> GetBookingsByHotelAsync(int hotelId, PagedRequestDto request)
        {
            try
            {
                if (request.PageNumber <= 0) request.PageNumber = 1;
                if (request.PageSize <= 0) request.PageSize = 10;

                var allBookings = await _bookingRepository.GetAllAsync();
                var filtered = allBookings.Where(b => b.HotelId == hotelId).ToList();
                var total = filtered.Count;

                var paged = filtered
                    .OrderByDescending(b => b.CheckIn)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var data = new List<BookingResponseDto>();

                foreach (var booking in paged)
                {
                    var hotel = await _hotelRepository.GetByIdAsync(booking.HotelId);
                    data.Add(MapToBookingDto(booking, 1, hotel?.HotelName ?? string.Empty));
                }

                return new PagedResponseDto<BookingResponseDto>
                {
                    Data = data,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalRecords = total
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingsByHotelAsync: {ex.Message}");
                throw;
            }
        }

        // GET PENDING BOOKINGS FOR HOTEL
        public async Task<List<BookingResponseDto>> GetPendingBookingsForHotelAsync(int hotelId)
        {
            try
            {
                var allBookings = await _bookingRepository.GetAllAsync();
                var pendingBookings = allBookings.Where(b => b.HotelId == hotelId && b.Status == "Pending").ToList();

                var result = new List<BookingResponseDto>();
                foreach (var booking in pendingBookings)
                {
                    var hotel = await _hotelRepository.GetByIdAsync(booking.HotelId);
                    result.Add(MapToBookingDto(booking, 1, hotel?.HotelName ?? string.Empty));
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPendingBookingsForHotelAsync: {ex.Message}");
                throw;
            }
        }

        // HELPER: Map Booking to DTO
        private static BookingResponseDto MapToBookingDto(Booking booking, int numberOfRooms, string hotelName)
        {
            return new BookingResponseDto
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                HotelId = booking.HotelId,
                HotelName = hotelName,
                RoomId = booking.RoomId,
                NumberOfRooms = numberOfRooms,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status
            };
        }
    }
}