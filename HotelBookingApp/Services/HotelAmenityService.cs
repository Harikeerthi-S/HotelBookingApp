using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using HotelBookingAppWebApi.Interfaces;

namespace HotelBookingApp.Services
{
    public class HotelAmenityService : IHotelAmenityService
    {
        private readonly IRepository<int, HotelAmenity> _hotelAmenityRepository;
        private readonly IRepository<int, Hotel> _hotelRepository;
        private readonly IRepository<int, Amenity> _amenityRepository;

        public HotelAmenityService(
            IRepository<int, HotelAmenity> hotelAmenityRepository,
            IRepository<int, Hotel> hotelRepository,
            IRepository<int, Amenity> amenityRepository)
        {
            _hotelAmenityRepository = hotelAmenityRepository ?? throw new ArgumentNullException(nameof(hotelAmenityRepository));
            _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
            _amenityRepository = amenityRepository ?? throw new ArgumentNullException(nameof(amenityRepository));
        }

        // ===============================
        // GET ALL HOTEL AMENITIES
        // ===============================
        public async Task<IEnumerable<HotelAmenityResponseDto>> GetAllAsync()
        {
            try
            {
                var allAmenities = await _hotelAmenityRepository.GetAllAsync();
                return allAmenities.Select(ha => new HotelAmenityResponseDto
                {
                    HotelAmenityId = ha.HotelAmenityId,
                    HotelId = ha.HotelId,
                    AmenityId = ha.AmenityId
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving hotel amenities.", ex);
            }
        }

        // ===============================
        // GET HOTEL AMENITY BY ID
        // ===============================
        public async Task<HotelAmenityResponseDto?> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _hotelAmenityRepository.GetByIdAsync(id);
                if (entity == null) return null;

                return new HotelAmenityResponseDto
                {
                    HotelAmenityId = entity.HotelAmenityId,
                    HotelId = entity.HotelId,
                    AmenityId = entity.AmenityId
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving HotelAmenity with ID {id}.", ex);
            }
        }

        // ===============================
        // CREATE HOTEL AMENITY
        // ===============================
        public async Task<HotelAmenityResponseDto> CreateAsync(HotelAmenityDto dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                // Validate hotel exists
                var hotel = await _hotelRepository.GetByIdAsync(dto.HotelId)
                             ?? throw new KeyNotFoundException("Hotel not found.");

                // Validate amenity exists
                var amenity = await _amenityRepository.GetByIdAsync(dto.AmenityId)
                              ?? throw new KeyNotFoundException("Amenity not found.");

                // Prevent duplicate mapping
                var allHotelAmenities = await _hotelAmenityRepository.GetAllAsync();
                if (allHotelAmenities.Any(x => x.HotelId == dto.HotelId && x.AmenityId == dto.AmenityId))
                    throw new InvalidOperationException("This amenity is already assigned to the hotel.");

                var entity = new HotelAmenity
                {
                    HotelId = dto.HotelId,
                    AmenityId = dto.AmenityId
                };

                var created = await _hotelAmenityRepository.AddAsync(entity);

                return new HotelAmenityResponseDto
                {
                    HotelAmenityId = created.HotelAmenityId,
                    HotelId = created.HotelId,
                    AmenityId = created.AmenityId
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating hotel amenity.", ex);
            }
        }

        // ===============================
        // DELETE HOTEL AMENITY
        // ===============================
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _hotelAmenityRepository.GetByIdAsync(id);
                if (entity == null) return false;

                await _hotelAmenityRepository.DeleteAsync(id);

                return true; // return true if deleted
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting HotelAmenity with ID {id}.", ex);
            }
        }
    }
}