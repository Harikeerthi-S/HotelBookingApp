using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Services
{
    public class HotelService : IHotelService
    {
        private readonly HotelBookingContext _context;

        public HotelService(HotelBookingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // CREATE HOTEL
        public async Task<HotelResponseDto> CreateHotelAsync(CreateHotelDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.HotelName))
                throw new ArgumentException("Hotel name is required.");
            if (dto.StarRating < 1 || dto.StarRating > 5)
                throw new ArgumentException("Star rating must be between 1 and 5.");

            try
            {
                var exists = await _context.Hotels
                    .AnyAsync(h => h.HotelName == dto.HotelName && h.Location == dto.Location);
                if (exists)
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

                _context.Hotels.Add(hotel);
                await _context.SaveChangesAsync();

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
            catch (DbUpdateException dbEx)
            {
                throw new InvalidOperationException("Database error while creating hotel.", dbEx);
            }
        }

        // GET HOTEL BY ID
        public async Task<HotelResponseDto?> GetHotelByIdAsync(int hotelId)
        {
            if (hotelId <= 0) throw new ArgumentException("HotelId must be greater than 0.");

            var hotel = await _context.Hotels
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.HotelId == hotelId && h.IsActive);

            if (hotel == null) return null;

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

        // PAGINATED HOTELS
        public async Task<PagedResponseDto<HotelResponseDto>> GetHotelsPagedAsync(PagedRequestDto request)
        {
            if (request.PageNumber <= 0 || request.PageSize <= 0)
                throw new ArgumentException("PageNumber and PageSize must be greater than 0.");

            var query = _context.Hotels.AsNoTracking().Where(h => h.IsActive);

            var totalRecords = await query.CountAsync();
            var hotels = await query
                .OrderByDescending(h => h.StarRating)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var data = hotels.Select(h => new HotelResponseDto
            {
                HotelId = h.HotelId,
                HotelName = h.HotelName,
                ImagePath = h.ImagePath,
                Location = h.Location,
                Address = h.Address,
                StarRating = h.StarRating,
                ContactNumber = h.ContactNumber
            }).ToList();

            return new PagedResponseDto<HotelResponseDto>
            {
                Data = data,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecords = totalRecords
            };
        }

        // SEARCH HOTELS BY LOCATION
        public async Task<IEnumerable<HotelResponseDto>> SearchHotelsAsync(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location is required.");

            var hotels = await _context.Hotels
                .AsNoTracking()
                .Where(h => h.IsActive && h.Location.ToLower().Contains(location.ToLower()))
                .ToListAsync();

            return hotels.Select(h => new HotelResponseDto
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

        // FILTER HOTELS WITH PAGINATION
        public async Task<PagedResponseDto<HotelResponseDto>> FilterHotelsPagedAsync(HotelFilterDto filter, PagedRequestDto request)
        {
            if (request.PageNumber <= 0 || request.PageSize <= 0)
                throw new ArgumentException("PageNumber and PageSize must be greater than 0.");

            var query = _context.Hotels.AsNoTracking().Where(h => h.IsActive).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Location))
                query = query.Where(h => h.Location.Contains(filter.Location));

            if (filter.MinRating.HasValue)
                query = query.Where(h => h.StarRating >= filter.MinRating.Value);

            if (filter.MinPrice.HasValue)
                query = query.Where(h => h.Rooms!.Any(r => r.PricePerNight >= filter.MinPrice.Value));

            if (filter.MaxPrice.HasValue)
                query = query.Where(h => h.Rooms!.Any(r => r.PricePerNight <= filter.MaxPrice.Value));

            if (filter.AmenityId.HasValue)
                query = query.Where(h => h.HotelAmenities!.Any(a => a.AmenityId == filter.AmenityId));

            var totalRecords = await query.CountAsync();
            var hotels = await query
                .OrderByDescending(h => h.StarRating)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var data = hotels.Select(h => new HotelResponseDto
            {
                HotelId = h.HotelId,
                HotelName = h.HotelName,
                ImagePath = h.ImagePath,
                Location = h.Location,
                Address = h.Address,
                StarRating = h.StarRating,
                ContactNumber = h.ContactNumber
            }).ToList();

            return new PagedResponseDto<HotelResponseDto>
            {
                Data = data,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecords = totalRecords
            };
        }

        // UPDATE HOTEL
        public async Task<HotelResponseDto?> UpdateHotelAsync(int hotelId, CreateHotelDto dto)
        {
            var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.HotelId == hotelId);
            if (hotel == null) return null;

            if (dto.StarRating < 1 || dto.StarRating > 5)
                throw new ArgumentException("Star rating must be between 1 and 5.");

            hotel.HotelName = dto.HotelName;
            hotel.ImagePath = dto.ImagePath;
            hotel.Location = dto.Location;
            hotel.Address = dto.Address;
            hotel.StarRating = dto.StarRating;
            hotel.ContactNumber = dto.ContactNumber;

            try
            {
                await _context.SaveChangesAsync();
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
            catch (DbUpdateException dbEx)
            {
                throw new InvalidOperationException("Database error while updating hotel.", dbEx);
            }
        }

        // SOFT DELETE
        public async Task<bool> DeactivateHotelAsync(int hotelId)
        {
            var hotel = await _context.Hotels.FindAsync(hotelId);
            if (hotel == null) return false;
            if (!hotel.IsActive) throw new InvalidOperationException("Hotel is already deactivated.");

            hotel.IsActive = false;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                throw new InvalidOperationException("Database error while deactivating hotel.", dbEx);
            }
        }
    }
}