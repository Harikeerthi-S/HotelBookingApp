using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using HotelBookingAppWebApi.Interfaces;

namespace HotelBookingApp.Services
{
    public class HotelService : IHotelService
    {
        private readonly IRepository<int, Hotel> _hotelRepository;

        public HotelService(IRepository<int, Hotel> hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        // ================= CREATE =================
        public async Task<HotelResponseDto> CreateHotelAsync(CreateHotelDto dto)
        {
            try
            {
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto));

                var hotel = new Hotel
                {
                    HotelName = dto.HotelName ?? "",
                    Location = dto.Location ?? "",
                    Address = dto.Address,
                    StarRating = dto.StarRating,
                    TotalRooms = dto.TotalRooms,
                    ContactNumber = dto.ContactNumber,
                    ImagePath = dto.ImagePath,
                    IsActive = true
                };

                var result = await _hotelRepository.AddAsync(hotel);

                return MapToDto(result!);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating hotel: {ex.Message}");
            }
        }

        // ================= GET BY ID =================
        public async Task<HotelResponseDto?> GetHotelByIdAsync(int hotelId)
        {
            try
            {
                var hotel = await _hotelRepository.GetByIdAsync(hotelId);

                if (hotel == null || !hotel.IsActive)
                    return null;

                return MapToDto(hotel);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching hotel: {ex.Message}");
            }
        }

        // ================= GET PAGED =================
        public async Task<PagedResponseDto<HotelResponseDto>> GetHotelsPagedAsync(PagedRequestDto request)
        {
            try
            {
                request ??= new PagedRequestDto();

                request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
                request.PageSize = request.PageSize <= 0 ? 10 : request.PageSize;

                var hotels = (await _hotelRepository.GetAllAsync()) ?? new List<Hotel>();

                var query = hotels.Where(h => h != null && h.IsActive);

                var total = query.Count();

                var data = query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(MapToDto)
                    .ToList();

                return new PagedResponseDto<HotelResponseDto>
                {
                    Data = data,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalRecords = total
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching hotels: {ex.Message}");
            }
        }

        // ================= FILTER =================
        public async Task<PagedResponseDto<HotelResponseDto>> FilterHotelsPagedAsync(
            HotelFilterDto filter,
            PagedRequestDto request)
        {
            try
            {
                filter ??= new HotelFilterDto();
                request ??= new PagedRequestDto();

                request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
                request.PageSize = request.PageSize <= 0 ? 10 : request.PageSize;

                var hotels = await _hotelRepository.GetAllAsync() ?? new List<Hotel>();

                var query = hotels
                    .Where(h => h != null && h.IsActive)
                    .AsQueryable();

                if (filter.HotelId.HasValue)
                    query = query.Where(h => h.HotelId == filter.HotelId.Value);

                if (!string.IsNullOrWhiteSpace(filter.Location))
                {
                    var loc = filter.Location.ToLower();
                    query = query.Where(h => (h.Location ?? "").ToLower().Contains(loc));
                }

                if (filter.MinRating.HasValue)
                    query = query.Where(h => h.StarRating >= filter.MinRating.Value);

                var total = query.Count();

                var data = query
                    .OrderByDescending(h => h.StarRating)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(MapToDto)
                    .ToList();

                return new PagedResponseDto<HotelResponseDto>
                {
                    Data = data,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalRecords = total
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error filtering hotels: {ex.Message}");
            }
        }

        // ================= SEARCH =================
        public async Task<IEnumerable<HotelResponseDto>> SearchHotelsAsync(string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(location))
                    return new List<HotelResponseDto>();

                var hotels = await _hotelRepository.GetAllAsync() ?? new List<Hotel>();

                return hotels
                    .Where(h => h.IsActive &&
                                (h.Location ?? "").ToLower().Contains(location.ToLower()))
                    .Select(MapToDto)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching hotels: {ex.Message}");
            }
        }

        // ================= UPDATE =================
        public async Task<HotelResponseDto?> UpdateHotelAsync(int hotelId, CreateHotelDto dto)
        {
            try
            {
                var hotel = await _hotelRepository.GetByIdAsync(hotelId);

                if (hotel == null || !hotel.IsActive)
                    return null;

                hotel.HotelName = dto.HotelName ?? hotel.HotelName;
                hotel.Location = dto.Location ?? hotel.Location;
                hotel.Address = dto.Address;
                hotel.StarRating = dto.StarRating;
                hotel.TotalRooms = dto.TotalRooms;
                hotel.ContactNumber = dto.ContactNumber;
                hotel.ImagePath = dto.ImagePath;

                var updated = await _hotelRepository.UpdateAsync(hotelId, hotel);

                return updated == null ? null : MapToDto(updated);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating hotel: {ex.Message}");
            }
        }

        // ================= DELETE =================
        public async Task<bool> DeactivateHotelAsync(int hotelId)
        {
            try
            {
                var hotel = await _hotelRepository.GetByIdAsync(hotelId);

                if (hotel == null)
                    return false;

                hotel.IsActive = false;

                await _hotelRepository.UpdateAsync(hotelId, hotel);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting hotel: {ex.Message}");
            }
        }

        // ================= MAPPER =================
        private static HotelResponseDto MapToDto(Hotel h)
        {
            return new HotelResponseDto
            {
                HotelId = h.HotelId,
                HotelName = h.HotelName ?? "",
                Location = h.Location ?? "",
                Address = h.Address,
                StarRating = h.StarRating,
                TotalRooms = h.TotalRooms,
                ContactNumber = h.ContactNumber,
                ImagePath = h.ImagePath
            };
        }
    }
}