using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Services
{
    public class AmenityService : IAmenityService
    {
        private readonly HotelBookingContext _context;

        public AmenityService(HotelBookingContext context)
        {
            _context = context;
        }

        // ==============================
        // GET ALL
        // ==============================
        public async Task<IEnumerable<AmenityResponseDto>> GetAllAsync()
        {
            var amenities = await _context.Amenities
                .AsNoTracking()
                .OrderBy(a => a.Name)
                .ToListAsync();

            return amenities.Select(a => new AmenityResponseDto
            {
                AmenityId = a.AmenityId,
                Name = a.Name,
                Description = a.Description,
                Icon = a.Icon
            });
        }

        // ==============================
        // GET BY ID
        // ==============================
        public async Task<AmenityResponseDto?> GetByIdAsync(int id)
        {
            var amenity = await _context.Amenities
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AmenityId == id);

            if (amenity == null)
                return null;

            return new AmenityResponseDto
            {
                AmenityId = amenity.AmenityId,
                Name = amenity.Name,
                Description = amenity.Description,
                Icon = amenity.Icon
            };
        }

        // ==============================
        // CREATE
        // ==============================
        public async Task<AmenityResponseDto> CreateAsync(CreateAmenityDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Amenity name is required.");

            var name = dto.Name.Trim();

            // Business Rule: Name must be unique
            bool exists = await _context.Amenities
                .AnyAsync(a => a.Name.ToLower() == name.ToLower());

            if (exists)
                throw new ArgumentException("Amenity with the same name already exists.");

            var amenity = new Amenity
            {
                Name = name,
                Description = dto.Description,
                Icon = dto.Icon
            };

            _context.Amenities.Add(amenity);
            await _context.SaveChangesAsync();

            return new AmenityResponseDto
            {
                AmenityId = amenity.AmenityId,
                Name = amenity.Name,
                Description = amenity.Description,
                Icon = amenity.Icon
            };
        }

        // ==============================
        // UPDATE
        // ==============================
        public async Task<bool> UpdateAsync(int id, CreateAmenityDto dto)
        {
            var amenity = await _context.Amenities.FindAsync(id);

            if (amenity == null)
                return false;

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Amenity name is required.");

            var name = dto.Name.Trim();

            // Prevent duplicate name
            bool duplicate = await _context.Amenities
                .AnyAsync(a => a.AmenityId != id &&
                               a.Name.ToLower() == name.ToLower());

            if (duplicate)
                throw new ArgumentException("Another amenity with the same name already exists.");

            amenity.Name = name;
            amenity.Description = dto.Description;
            amenity.Icon = dto.Icon;

            await _context.SaveChangesAsync();
            return true;
        }

        // ==============================
        // DELETE
        // ==============================
        public async Task<bool> DeleteAsync(int id)
        {
            var amenity = await _context.Amenities
                .Include(a => a.HotelAmenities)
                .FirstOrDefaultAsync(a => a.AmenityId == id);

            if (amenity == null)
                return false;

            // Business Rule:
            // Do NOT allow delete if assigned to any hotel
            if (amenity.HotelAmenities != null && amenity.HotelAmenities.Any())
                throw new InvalidOperationException("Cannot delete amenity assigned to hotels.");

            _context.Amenities.Remove(amenity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}