using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Services
{
    public class RoomService : IRoomService
    {
        private readonly HotelBookingContext _context;

        public RoomService(HotelBookingContext context)
        {
            _context = context;
        }

        public async Task<RoomResponseDto> CreateRoomAsync(CreateRoomDto dto)
        {
            if (dto.PricePerNight <= 0) throw new ArgumentException("Price per night must be greater than zero.");
            if (dto.Capacity <= 0) throw new ArgumentException("Capacity must be greater than zero.");

            var hotelExists = await _context.Hotels.AnyAsync(h => h.HotelId == dto.HotelId && h.IsActive);
            if (!hotelExists) throw new InvalidOperationException("Hotel does not exist.");

            var roomExists = await _context.Rooms.AnyAsync(r => r.HotelId == dto.HotelId && r.RoomNumber == dto.RoomNumber);
            if (roomExists) throw new InvalidOperationException("Room number already exists in this hotel.");

            var room = new Room
            {
                HotelId = dto.HotelId,
                RoomNumber = dto.RoomNumber,
                RoomType = dto.RoomType,
                PricePerNight = dto.PricePerNight,
                Capacity = dto.Capacity,
                IsAvailable = true
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return new RoomResponseDto
            {
                RoomId = room.RoomId,
                HotelId = room.HotelId,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                PricePerNight = room.PricePerNight,
                Capacity = room.Capacity,
                IsAvailable = room.IsAvailable
            };
        }

        public async Task<RoomResponseDto?> GetRoomByIdAsync(int roomId)
        {
            var room = await _context.Rooms.AsNoTracking()
                .FirstOrDefaultAsync(r => r.RoomId == roomId && r.IsAvailable);

            if (room == null) return null;

            return new RoomResponseDto
            {
                RoomId = room.RoomId,
                HotelId = room.HotelId,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                PricePerNight = room.PricePerNight,
                Capacity = room.Capacity,
                IsAvailable = room.IsAvailable
            };
        }

        public async Task<IEnumerable<RoomResponseDto>> GetAllRoomsAsync(int? hotelId = null)
        {
            var query = _context.Rooms.AsNoTracking().Where(r => r.IsAvailable);
            if (hotelId.HasValue) query = query.Where(r => r.HotelId == hotelId.Value);

            return await query
                .OrderBy(r => r.RoomNumber)
                .Select(r => new RoomResponseDto
                {
                    RoomId = r.RoomId,
                    HotelId = r.HotelId,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.RoomType,
                    PricePerNight = r.PricePerNight,
                    Capacity = r.Capacity,
                    IsAvailable = r.IsAvailable
                })
                .ToListAsync();
        }

        public async Task<RoomResponseDto?> UpdateRoomAsync(int roomId, CreateRoomDto dto)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId);
            if (room == null) return null;

            if (dto.PricePerNight <= 0) throw new ArgumentException("Price per night must be greater than zero.");
            if (dto.Capacity <= 0) throw new ArgumentException("Capacity must be greater than zero.");

            var duplicateRoom = await _context.Rooms.AnyAsync(r => r.RoomId != roomId &&
                                                                  r.HotelId == dto.HotelId &&
                                                                  r.RoomNumber == dto.RoomNumber);
            if (duplicateRoom) throw new InvalidOperationException("Room number already exists in this hotel.");

            room.HotelId = dto.HotelId;
            room.RoomNumber = dto.RoomNumber;
            room.RoomType = dto.RoomType;
            room.PricePerNight = dto.PricePerNight;
            room.Capacity = dto.Capacity;

            await _context.SaveChangesAsync();

            return new RoomResponseDto
            {
                RoomId = room.RoomId,
                HotelId = room.HotelId,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                PricePerNight = room.PricePerNight,
                Capacity = room.Capacity,
                IsAvailable = room.IsAvailable
            };
        }

        public async Task<bool> DeactivateRoomAsync(int roomId)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId);
            if (room == null) return false;
            if (!room.IsAvailable) throw new InvalidOperationException("Room is already deactivated.");

            room.IsAvailable = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RoomResponseDto>> GetRoomsFilteredAsync(RoomFilterDto filter)
        {
            var query = ApplyFilter(_context.Rooms.AsNoTracking(), filter);

            return await query.OrderBy(r => r.RoomNumber)
                .Select(r => new RoomResponseDto
                {
                    RoomId = r.RoomId,
                    HotelId = r.HotelId,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.RoomType,
                    PricePerNight = r.PricePerNight,
                    Capacity = r.Capacity,
                    IsAvailable = r.IsAvailable
                })
                .ToListAsync();
        }

        public async Task<PagedResponseDto<RoomResponseDto>> GetRoomsFilteredPagedAsync(RoomFilterDto filter, PagedRequestDto pageRequest)
        {
            if (pageRequest.PageNumber <= 0) pageRequest.PageNumber = 1;
            if (pageRequest.PageSize <= 0) pageRequest.PageSize = 10;

            var query = ApplyFilter(_context.Rooms.AsNoTracking(), filter);
            var totalRecords = await query.CountAsync();

            var data = await query.OrderBy(r => r.RoomNumber)
                .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize)
                .Select(r => new RoomResponseDto
                {
                    RoomId = r.RoomId,
                    HotelId = r.HotelId,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.RoomType,
                    PricePerNight = r.PricePerNight,
                    Capacity = r.Capacity,
                    IsAvailable = r.IsAvailable
                })
                .ToListAsync();

            return new PagedResponseDto<RoomResponseDto>
            {
                Data = data,
                PageNumber = pageRequest.PageNumber,
                PageSize = pageRequest.PageSize,
                TotalRecords = totalRecords
            };
        }

        // ======================
        // PRIVATE FILTER METHOD
        // ======================
        private static IQueryable<Room> ApplyFilter(IQueryable<Room> query, RoomFilterDto filter)
        {
            if (filter.OnlyAvailable) query = query.Where(r => r.IsAvailable);
            if (filter.HotelId.HasValue) query = query.Where(r => r.HotelId == filter.HotelId.Value);
            if (!string.IsNullOrWhiteSpace(filter.RoomType)) query = query.Where(r => r.RoomType.ToLower() == filter.RoomType.ToLower());
            if (filter.MinPrice.HasValue) query = query.Where(r => r.PricePerNight >= filter.MinPrice.Value);
            if (filter.MaxPrice.HasValue) query = query.Where(r => r.PricePerNight <= filter.MaxPrice.Value);
            if (filter.MinCapacity.HasValue) query = query.Where(r => r.Capacity >= filter.MinCapacity.Value);
            if (filter.MaxCapacity.HasValue) query = query.Where(r => r.Capacity <= filter.MaxCapacity.Value);
            return query;
        }
    }
}