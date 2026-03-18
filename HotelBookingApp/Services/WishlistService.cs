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

        public async Task<WishlistResponseDto> AddToWishlistAsync(WishlistDto dto)
        {
            try
            {
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto));

                var user = await _userRepository.GetByIdAsync(dto.UserId)
                           ?? throw new Exception("User not found");

                var hotel = await _hotelRepository.GetByIdAsync(dto.HotelId)
                            ?? throw new Exception("Hotel not found");

                var wishlistItems = await _wishlistRepository.GetAllAsync()
                                    ?? Enumerable.Empty<Wishlist>();

                if (wishlistItems.Any(w => w.UserId == dto.UserId && w.HotelId == dto.HotelId))
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
            catch
            {
                throw; // preserve original stack trace
            }
        }

        public async Task<IEnumerable<WishlistResponseDto>> GetUserWishlistAsync(int userId)
        {
            try
            {
                var wishlistItems = await _wishlistRepository.GetAllAsync()
                                    ?? Enumerable.Empty<Wishlist>();

                return wishlistItems
                    .Where(w => w.UserId == userId)
                    .Select(w => new WishlistResponseDto
                    {
                        WishlistId = w.WishlistId,
                        UserId = w.UserId,
                        HotelId = w.HotelId
                    });
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> RemoveFromWishlistAsync(int wishlistId)
        {
            try
            {
                var deleted = await _wishlistRepository.DeleteAsync(wishlistId);
                return deleted != null;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> RemoveByUserAndHotelAsync(int userId, int hotelId)
        {
            try
            {
                var wishlistItems = await _wishlistRepository.GetAllAsync()
                                    ?? Enumerable.Empty<Wishlist>();

                var item = wishlistItems.FirstOrDefault(w =>
                    w.UserId == userId && w.HotelId == hotelId);

                if (item == null)
                    return false;

                await _wishlistRepository.DeleteAsync(item.WishlistId);
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}