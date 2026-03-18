using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using HotelBookingAppWebApi.Interfaces;

namespace HotelBookingApp.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRepository<int, Room> _roomRepository;
        private readonly IRepository<int, Hotel> _hotelRepository;

        public RoomService(
            IRepository<int, Room> roomRepository,
            IRepository<int, Hotel> hotelRepository)
        {
            _roomRepository = roomRepository;
            _hotelRepository = hotelRepository;
        }

        // ======================================
        // CREATE ROOM
        // ======================================
        public async Task<RoomResponseDto> CreateRoomAsync(CreateRoomDto dto)
        {
            try
            {
                if (dto.PricePerNight <= 0)
                    throw new ArgumentException("Price per night must be greater than zero.");

                if (dto.Capacity <= 0)
                    throw new ArgumentException("Capacity must be greater than zero.");

                var hotel = await _hotelRepository.GetByIdAsync(dto.HotelId);
                if (hotel == null || !hotel.IsActive)
                    throw new Exception("Hotel does not exist.");

                var rooms = await _roomRepository.GetAllAsync();

                if (rooms.Any(r => r.HotelId == dto.HotelId && r.RoomNumber == dto.RoomNumber))
                    throw new Exception("Room number already exists in this hotel.");

                var room = new Room
                {
                    HotelId = dto.HotelId,
                    RoomNumber = dto.RoomNumber,
                    RoomType = dto.RoomType,
                    PricePerNight = dto.PricePerNight,
                    Capacity = dto.Capacity,
                    IsAvailable = true
                };

                var created = await _roomRepository.AddAsync(room);

                return MapToDto(created);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating room: {ex.Message}");
            }
        }

        // ======================================
        // GET ROOM BY ID
        // ======================================
        public async Task<RoomResponseDto?> GetRoomByIdAsync(int roomId)
        {
            try
            {
                var room = await _roomRepository.GetByIdAsync(roomId);

                if (room == null || !room.IsAvailable)
                    return null;

                return MapToDto(room);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving room: {ex.Message}");
            }
        }

        // ======================================
        // GET ALL ROOMS
        // ======================================
        public async Task<IEnumerable<RoomResponseDto>> GetAllRoomsAsync(int? hotelId = null)
        {
            try
            {
                var rooms = await _roomRepository.GetAllAsync();

                var query = rooms.Where(r => r.IsAvailable);

                if (hotelId.HasValue)
                    query = query.Where(r => r.HotelId == hotelId.Value);

                return query
                    .OrderBy(r => r.RoomNumber)
                    .Select(MapToDto)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving rooms: {ex.Message}");
            }
        }

        // ======================================
        // UPDATE ROOM
        // ======================================
        public async Task<RoomResponseDto?> UpdateRoomAsync(int roomId, CreateRoomDto dto)
        {
            try
            {
                var room = await _roomRepository.GetByIdAsync(roomId);

                if (room == null)
                    return null;

                if (dto.PricePerNight <= 0)
                    throw new ArgumentException("Price per night must be greater than zero.");

                if (dto.Capacity <= 0)
                    throw new ArgumentException("Capacity must be greater than zero.");

                var rooms = await _roomRepository.GetAllAsync();

                var duplicateRoom = rooms.Any(r =>
                    r.RoomId != roomId &&
                    r.HotelId == dto.HotelId &&
                    r.RoomNumber == dto.RoomNumber);

                if (duplicateRoom)
                    throw new Exception("Room number already exists in this hotel.");

                room.HotelId = dto.HotelId;
                room.RoomNumber = dto.RoomNumber;
                room.RoomType = dto.RoomType;
                room.PricePerNight = dto.PricePerNight;
                room.Capacity = dto.Capacity;

                var updated = await _roomRepository.UpdateAsync(roomId, room);

                return MapToDto(updated!);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating room: {ex.Message}");
            }
        }

        // ======================================
        // DEACTIVATE ROOM
        // ======================================
        public async Task<bool> DeactivateRoomAsync(int roomId)
        {
            try
            {
                var room = await _roomRepository.GetByIdAsync(roomId);

                if (room == null)
                    return false;

                if (!room.IsAvailable)
                    throw new Exception("Room is already deactivated.");

                room.IsAvailable = false;

                await _roomRepository.UpdateAsync(roomId, room);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deactivating room: {ex.Message}");
            }
        }

        // ======================================
        // FILTER ROOMS
        // ======================================
        public async Task<IEnumerable<RoomResponseDto>> GetRoomsFilteredAsync(RoomFilterDto filter)
        {
            try
            {
                var rooms = await _roomRepository.GetAllAsync();

                var query = ApplyFilter(rooms.AsQueryable(), filter);

                return query
                    .OrderBy(r => r.RoomNumber)
                    .Select(MapToDto)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error filtering rooms: {ex.Message}");
            }
        }

        // ======================================
        // FILTER + PAGINATION (POST)
        // ======================================
        public async Task<PagedResponseDto<RoomResponseDto>> GetRoomsFilteredPagedAsync(
            RoomFilterDto filter,
            PagedRequestDto pageRequest)
        {
            try
            {
                if (pageRequest.PageNumber <= 0)
                    pageRequest.PageNumber = 1;

                if (pageRequest.PageSize <= 0)
                    pageRequest.PageSize = 10;

                var rooms = await _roomRepository.GetAllAsync();

                var query = ApplyFilter(rooms.AsQueryable(), filter);

                var totalRecords = query.Count();

                var data = query
                    .OrderBy(r => r.RoomNumber)
                    .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                    .Take(pageRequest.PageSize)
                    .Select(MapToDto)
                    .ToList();

                return new PagedResponseDto<RoomResponseDto>
                {
                    Data = data,
                    PageNumber = pageRequest.PageNumber,
                    PageSize = pageRequest.PageSize,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paginated rooms: {ex.Message}");
            }
        }

        // ======================================
        // FILTER METHOD
        // ======================================
        private static IQueryable<Room> ApplyFilter(IQueryable<Room> query, RoomFilterDto filter)
        {
            if (filter.OnlyAvailable)
                query = query.Where(r => r.IsAvailable);

            if (filter.HotelId.HasValue)
                query = query.Where(r => r.HotelId == filter.HotelId.Value);

            if (!string.IsNullOrWhiteSpace(filter.RoomType))
                query = query.Where(r => r.RoomType.ToLower() == filter.RoomType.ToLower());

            if (filter.MinPrice.HasValue)
                query = query.Where(r => r.PricePerNight >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(r => r.PricePerNight <= filter.MaxPrice.Value);

            if (filter.MinCapacity.HasValue)
                query = query.Where(r => r.Capacity >= filter.MinCapacity.Value);

            if (filter.MaxCapacity.HasValue)
                query = query.Where(r => r.Capacity <= filter.MaxCapacity.Value);

            return query;
        }

        // ======================================
        // DTO MAPPER
        // ======================================
        private static RoomResponseDto MapToDto(Room room)
        {
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
    }
}