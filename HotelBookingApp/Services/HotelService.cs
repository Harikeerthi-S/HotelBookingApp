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
            _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
        }

        // ===============================
        // CREATE HOTEL
        // ===============================
        public async Task<HotelResponseDto> CreateHotelAsync(CreateHotelDto dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));
                if (string.IsNullOrWhiteSpace(dto.HotelName)) throw new ArgumentException("Hotel name is required.");
                if (dto.StarRating < 1 || dto.StarRating > 5) throw new ArgumentException("Star rating must be between 1 and 5.");

                var allHotels = await _hotelRepository.GetAllAsync();
                if (allHotels.Any(h => h.HotelName == dto.HotelName && h.Location == dto.Location))
                    throw new InvalidOperationException("Hotel already exists at this location.");

                var hotel = new Hotel
                {
                    HotelName = dto.HotelName,
                    ImagePath = dto.ImagePath,
                    Location = dto.Location,
                    Address = dto.Address,
                    StarRating = dto.StarRating,
                    ContactNumber = dto.ContactNumber,
                    IsActive = true
                };

                var createdHotel = await _hotelRepository.AddAsync(hotel);

                return new HotelResponseDto
                {
                    HotelId = createdHotel.HotelId,
                    HotelName = createdHotel.HotelName,
                    ImagePath = createdHotel.ImagePath,
                    Location = createdHotel.Location,
                    Address = createdHotel.Address,
                    StarRating = createdHotel.StarRating,
                    ContactNumber = createdHotel.ContactNumber
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating hotel: {ex.Message}");
            }
        }

        // ===============================
        // GET HOTEL BY ID
        // ===============================
        public async Task<HotelResponseDto?> GetHotelByIdAsync(int hotelId)
        {
            try
            {
                if (hotelId <= 0) throw new ArgumentException("HotelId must be greater than 0.");

                var hotel = await _hotelRepository.GetByIdAsync(hotelId);
                if (hotel == null || !hotel.IsActive) return null;

                return new HotelResponseDto
                {
                    HotelId = hotel.HotelId,
                    HotelName = hotel.HotelName,
                    ImagePath = hotel.ImagePath,
                    Location = hotel.Location,
                    Address = hotel.Address,
                    StarRating = hotel.StarRating,
                    ContactNumber = hotel.ContactNumber
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving hotel: {ex.Message}");
            }
        }

        // ===============================
        // GET HOTELS PAGED
        // ===============================
        public async Task<PagedResponseDto<HotelResponseDto>> GetHotelsPagedAsync(PagedRequestDto request)
        {
            try
            {
                if (request.PageNumber <= 0) request.PageNumber = 1;
                if (request.PageSize <= 0) request.PageSize = 10;

                var allHotels = (await _hotelRepository.GetAllAsync())
                                .Where(h => h.IsActive)
                                .OrderByDescending(h => h.StarRating)
                                .ToList();

                var totalRecords = allHotels.Count;

                var pagedHotels = allHotels
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(h => new HotelResponseDto
                    {
                        HotelId = h.HotelId,
                        HotelName = h.HotelName,
                        ImagePath = h.ImagePath,
                        Location = h.Location,
                        Address = h.Address,
                        StarRating = h.StarRating,
                        ContactNumber = h.ContactNumber
                    })
                    .ToList();

                return new PagedResponseDto<HotelResponseDto>
                {
                    Data = pagedHotels,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paged hotels: {ex.Message}");
            }
        }

        // ===============================
        // FILTER HOTELS WITH PAGINATION
        // ===============================
        public async Task<PagedResponseDto<HotelResponseDto>> FilterHotelsPagedAsync(HotelFilterDto filter, PagedRequestDto request)
        {
            try
            {
                if (request.PageNumber <= 0) request.PageNumber = 1;
                if (request.PageSize <= 0) request.PageSize = 10;

                var allHotels = await _hotelRepository.GetAllAsync();
                var filteredHotels = allHotels.Where(h => h.IsActive).AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.Location))
                    filteredHotels = filteredHotels.Where(h => h.Location.Contains(filter.Location, StringComparison.OrdinalIgnoreCase));

                if (filter.MinRating.HasValue)
                    filteredHotels = filteredHotels.Where(h => h.StarRating >= filter.MinRating.Value);

                if (filter.MinPrice.HasValue)
                    filteredHotels = filteredHotels.Where(h => h.Rooms != null && h.Rooms.Any(r => r.PricePerNight >= filter.MinPrice.Value));

                if (filter.MaxPrice.HasValue)
                    filteredHotels = filteredHotels.Where(h => h.Rooms != null && h.Rooms.Any(r => r.PricePerNight <= filter.MaxPrice.Value));

                if (filter.AmenityId.HasValue)
                    filteredHotels = filteredHotels.Where(h => h.HotelAmenities != null && h.HotelAmenities.Any(a => a.AmenityId == filter.AmenityId.Value));

                var totalRecords = filteredHotels.Count();

                var pagedHotels = filteredHotels
                    .OrderByDescending(h => h.StarRating)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(h => new HotelResponseDto
                    {
                        HotelId = h.HotelId,
                        HotelName = h.HotelName,
                        ImagePath = h.ImagePath,
                        Location = h.Location,
                        Address = h.Address,
                        StarRating = h.StarRating,
                        ContactNumber = h.ContactNumber
                    })
                    .ToList();

                return new PagedResponseDto<HotelResponseDto>
                {
                    Data = pagedHotels,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error filtering hotels: {ex.Message}");
            }
        }

        // ===============================
        // SEARCH HOTELS
        // ===============================
        public async Task<IEnumerable<HotelResponseDto>> SearchHotelsAsync(string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(location)) throw new ArgumentException("Location is required.");

                var allHotels = await _hotelRepository.GetAllAsync();
                return allHotels
                    .Where(h => h.IsActive && h.Location.Contains(location, StringComparison.OrdinalIgnoreCase))
                    .Select(h => new HotelResponseDto
                    {
                        HotelId = h.HotelId,
                        HotelName = h.HotelName,
                        ImagePath = h.ImagePath,
                        Location = h.Location,
                        Address = h.Address,
                        StarRating = h.StarRating,
                        ContactNumber = h.ContactNumber
                    });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching hotels: {ex.Message}");
            }
        }

        // ===============================
        // UPDATE HOTEL
        // ===============================
        public async Task<HotelResponseDto?> UpdateHotelAsync(int hotelId, CreateHotelDto dto)
        {
            try
            {
                var hotel = await _hotelRepository.GetByIdAsync(hotelId);
                if (hotel == null) return null;

                hotel.HotelName = dto.HotelName;
                hotel.ImagePath = dto.ImagePath;
                hotel.Location = dto.Location;
                hotel.Address = dto.Address;
                hotel.StarRating = dto.StarRating;
                hotel.ContactNumber = dto.ContactNumber;

                var updatedHotel = await _hotelRepository.UpdateAsync(hotelId, hotel);
                if (updatedHotel == null)
                    throw new InvalidOperationException($"Failed to update hotel with ID {hotelId}.");

                return new HotelResponseDto
                {
                    HotelId = updatedHotel.HotelId,
                    HotelName = updatedHotel.HotelName,
                    ImagePath = updatedHotel.ImagePath,
                    Location = updatedHotel.Location,
                    Address = updatedHotel.Address,
                    StarRating = updatedHotel.StarRating,
                    ContactNumber = updatedHotel.ContactNumber
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating hotel: {ex.Message}");
            }
        }

        // ===============================
        // DEACTIVATE HOTEL
        // ===============================
        public async Task<bool> DeactivateHotelAsync(int hotelId)
        {
            try
            {
                var hotel = await _hotelRepository.GetByIdAsync(hotelId);
                if (hotel == null) return false;
                if (!hotel.IsActive) throw new InvalidOperationException("Hotel is already deactivated.");

                hotel.IsActive = false;
                await _hotelRepository.UpdateAsync(hotelId, hotel);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deactivating hotel: {ex.Message}");
            }
        }
    }
}