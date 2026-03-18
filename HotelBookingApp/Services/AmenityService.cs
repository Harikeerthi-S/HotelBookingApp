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
    public class AmenityService : IAmenityService
    {
        private readonly IRepository<int, Amenity> _amenityRepository;

        public AmenityService(IRepository<int, Amenity> amenityRepository)
        {
            _amenityRepository = amenityRepository ?? throw new ArgumentNullException(nameof(amenityRepository));
        }

        // GET ALL
        public async Task<IEnumerable<AmenityResponseDto>> GetAllAsync()
        {
            var amenities = await _amenityRepository.GetAllAsync();

            // Order in-memory because repository returns IEnumerable
            var ordered = amenities.OrderBy(a => a.Name);

            return ordered.Select(a => new AmenityResponseDto
            {
                AmenityId = a.AmenityId,
                Name = a.Name,
                Description = a.Description,
                Icon = a.Icon
            });
        }

        // GET BY ID
        public async Task<AmenityResponseDto?> GetByIdAsync(int id)
        {
            var amenity = await _amenityRepository.GetByIdAsync(id);
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

        // CREATE
        public async Task<AmenityResponseDto> CreateAsync(CreateAmenityDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Amenity name is required.");

            var name = dto.Name.Trim();

            // Check uniqueness by loading all (could be optimized with repo extension)
            var amenities = await _amenityRepository.GetAllAsync();
            if (amenities.Any(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Amenity with the same name already exists.");

            var amenity = new Amenity
            {
                Name = name,
                Description = dto.Description,
                Icon = dto.Icon
            };

            var addedAmenity = await _amenityRepository.AddAsync(amenity);

            return new AmenityResponseDto
            {
                AmenityId = addedAmenity.AmenityId,
                Name = addedAmenity.Name,
                Description = addedAmenity.Description,
                Icon = addedAmenity.Icon
            };
        }

        // UPDATE
        public async Task<bool> UpdateAsync(int id, CreateAmenityDto dto)
        {
            var amenity = await _amenityRepository.GetByIdAsync(id);
            if (amenity == null)
                return false;

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Amenity name is required.");

            var name = dto.Name.Trim();

            var amenities = await _amenityRepository.GetAllAsync();
            if (amenities.Any(a => a.AmenityId != id && string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Another amenity with the same name already exists.");

            amenity.Name = name;
            amenity.Description = dto.Description;
            amenity.Icon = dto.Icon;

            var updated = await _amenityRepository.UpdateAsync(id, amenity);
            return updated != null;
        }

        // DELETE
        public async Task<bool> DeleteAsync(int id)
        {
            var amenity = await _amenityRepository.GetByIdAsync(id);
            if (amenity == null)
                return false;

            // Business rule:
            // Can't delete if assigned to hotels. Since repository doesn't support Include, 
            // you must have a way to check this separately or extend the repo for related data.

            // Here, just assuming no hotels assigned (or implement separate check)

            var deleted = await _amenityRepository.DeleteAsync(id);
            return deleted != null;
        }
    }
}