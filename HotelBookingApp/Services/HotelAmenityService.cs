using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Services
{
    public class HotelAmenityService : IHotelAmenityService
    {
        private readonly HotelBookingContext _context;

        public HotelAmenityService(HotelBookingContext context)
        {
            _context = context;
        }

        // ======================================
        // GET ALL
        // ======================================
        public async Task<IEnumerable<HotelAmenityResponseDto>> GetAllAsync()
        {
            try
            {
                var data = await _context.HotelAmenities
                    .AsNoTracking()
                    .ToListAsync();

                return data.Select(ha => new HotelAmenityResponseDto
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

        // ======================================
        // GET BY ID
        // ======================================
        public async Task<HotelAmenityResponseDto?> GetByIdAsync(int id)
        {
            try
            {
                var ha = await _context.HotelAmenities
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.HotelAmenityId == id);

                if (ha == null)
                    return null;

                return new HotelAmenityResponseDto
                {
                    HotelAmenityId = ha.HotelAmenityId,
                    HotelId = ha.HotelId,
                    AmenityId = ha.AmenityId
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving HotelAmenity with ID {id}.", ex);
            }
        }

        // ======================================
        // CREATE (BUSINESS LOGIC)
        // ======================================
        public async Task<HotelAmenityResponseDto> CreateAsync(HotelAmenityDto dto)
        {
            try
            {
                // 🔹 1. Validate Hotel Exists
                var hotelExists = await _context.Hotels
                    .AnyAsync(h => h.HotelId == dto.HotelId);

                if (!hotelExists)
                    throw new KeyNotFoundException("Hotel not found.");

                // 🔹 2. Validate Amenity Exists
                var amenityExists = await _context.Amenities
                    .AnyAsync(a => a.AmenityId == dto.AmenityId);

                if (!amenityExists)
                    throw new KeyNotFoundException("Amenity not found.");

                // 🔹 3. Prevent Duplicate Mapping
                var alreadyExists = await _context.HotelAmenities
                    .AnyAsync(x => x.HotelId == dto.HotelId &&
                                   x.AmenityId == dto.AmenityId);

                if (alreadyExists)
                    throw new InvalidOperationException("This amenity is already assigned to the hotel.");

                var entity = new HotelAmenity
                {
                    HotelId = dto.HotelId,
                    AmenityId = dto.AmenityId
                };

                _context.HotelAmenities.Add(entity);
                await _context.SaveChangesAsync();

                return new HotelAmenityResponseDto
                {
                    HotelAmenityId = entity.HotelAmenityId,
                    HotelId = entity.HotelId,
                    AmenityId = entity.AmenityId
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating hotel amenity.", ex);
            }
        }

        // ======================================
        // DELETE
        // ======================================
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.HotelAmenities
                    .FirstOrDefaultAsync(x => x.HotelAmenityId == id);

                if (entity == null)
                    return false;

                _context.HotelAmenities.Remove(entity);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting HotelAmenity with ID {id}.", ex);
            }
        }
    }
}