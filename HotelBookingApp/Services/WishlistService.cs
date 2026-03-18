using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using HotelBookingAppWebApi.Interfaces;

namespace HotelBookingApp.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IRepository<int, Wishlist> _wishlistRepository;
        private readonly IRepository<int, User> _userRepository;
        private readonly IRepository<int, Hotel> _hotelRepository;

        public WishlistService(
            IRepository<int, Wishlist> wishlistRepository,
            IRepository<int, User> userRepository,
            IRepository<int, Hotel> hotelRepository)
        {
            _wishlistRepository = wishlistRepository;
            _userRepository = userRepository;
            _hotelRepository = hotelRepository;
        }

        // =====================================
        // ADD TO WISHLIST
        // =====================================
        public async Task<WishlistResponseDto> AddToWishlistAsync(WishlistDto dto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(dto.UserId);
                if (user == null)
                    throw new Exception("User not found");

                var hotel = await _hotelRepository.GetByIdAsync(dto.HotelId);
                if (hotel == null)
                    throw new Exception("Hotel not found");

                var wishlistItems = await _wishlistRepository.GetAllAsync();

                var exists = wishlistItems.Any(w =>
                    w.UserId == dto.UserId &&
                    w.HotelId == dto.HotelId);

                if (exists)
                    throw new Exception("Hotel already exists in wishlist");

                var wishlist = new Wishlist
                {
                    UserId = dto.UserId,
                    HotelId = dto.HotelId
                };

                var created = await _wishlistRepository.AddAsync(wishlist);

                return new WishlistResponseDto
                {
                    WishlistId = created.WishlistId,
                    UserId = created.UserId,
                    HotelId = created.HotelId
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding hotel to wishlist: {ex.Message}");
            }
        }

        // =====================================
        // GET USER WISHLIST
        // =====================================
        public async Task<IEnumerable<WishlistResponseDto>> GetUserWishlistAsync(int userId)
        {
            try
            {
                var wishlistItems = await _wishlistRepository.GetAllAsync();

                return wishlistItems
                    .Where(w => w.UserId == userId)
                    .Select(w => new WishlistResponseDto
                    {
                        WishlistId = w.WishlistId,
                        UserId = w.UserId,
                        HotelId = w.HotelId
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving wishlist: {ex.Message}");
            }
        }

        // =====================================
        // REMOVE BY WISHLIST ID
        // =====================================
        public async Task<bool> RemoveFromWishlistAsync(int wishlistId)
        {
            try
            {
                var deleted = await _wishlistRepository.DeleteAsync(wishlistId);

                if (deleted == null)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error removing wishlist item: {ex.Message}");
            }
        }

        // =====================================
        // REMOVE BY USER + HOTEL
        // =====================================
        public async Task<bool> RemoveByUserAndHotelAsync(int userId, int hotelId)
        {
            try
            {
                var wishlistItems = await _wishlistRepository.GetAllAsync();

                var item = wishlistItems.FirstOrDefault(w =>
                    w.UserId == userId &&
                    w.HotelId == hotelId);

                if (item == null)
                    return false;

                await _wishlistRepository.DeleteAsync(item.WishlistId);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error removing wishlist item: {ex.Message}");
            }
        }
    }
}