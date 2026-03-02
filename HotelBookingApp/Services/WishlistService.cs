using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly HotelBookingContext _context;

        public WishlistService(HotelBookingContext context)
        {
            _context = context;
        }

        // ==========================================
        // ADD TO WISHLIST
        // ==========================================
        public async Task<WishlistResponseDto> AddToWishlistAsync(WishlistDto dto)
        {
            try
            {
                // Validate User
                var userExists = await _context.Users
                    .AnyAsync(u => u.UserId == dto.UserId);

                if (!userExists)
                    throw new Exception("User not found.");

                // Validate Hotel
                var hotelExists = await _context.Hotels
                    .AnyAsync(h => h.HotelId == dto.HotelId);

                if (!hotelExists)
                    throw new Exception("Hotel not found.");

                // Prevent duplicate wishlist entry
                var alreadyExists = await _context.Wishlists
                    .AnyAsync(w => w.UserId == dto.UserId && w.HotelId == dto.HotelId);

                if (alreadyExists)
                    throw new Exception("Hotel already exists in wishlist.");

                var wishlist = new Wishlist
                {
                    UserId = dto.UserId,
                    HotelId = dto.HotelId
                };

                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();

                return new WishlistResponseDto
                {
                    WishlistId = wishlist.WishlistId,
                    UserId = wishlist.UserId,
                    HotelId = wishlist.HotelId
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding to wishlist: {ex.Message}");
            }
        }

        // ==========================================
        // GET USER WISHLIST
        // ==========================================
        public async Task<IEnumerable<WishlistResponseDto>> GetUserWishlistAsync(int userId)
        {
            try
            {
                return await _context.Wishlists
                    .Where(w => w.UserId == userId)
                    .Select(w => new WishlistResponseDto
                    {
                        WishlistId = w.WishlistId,
                        UserId = w.UserId,
                        HotelId = w.HotelId
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving wishlist: {ex.Message}");
            }
        }

        // ==========================================
        // REMOVE BY WISHLIST ID
        // ==========================================
        public async Task<bool> RemoveFromWishlistAsync(int wishlistId)
        {
            try
            {
                var wishlist = await _context.Wishlists
                    .FirstOrDefaultAsync(w => w.WishlistId == wishlistId);

                if (wishlist == null)
                    return false;

                _context.Wishlists.Remove(wishlist);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error removing wishlist item: {ex.Message}");
            }
        }

        // ==========================================
        // REMOVE BY USER + HOTEL (Safer Option)
        // ==========================================
        public async Task<bool> RemoveByUserAndHotelAsync(int userId, int hotelId)
        {
            try
            {
                var wishlist = await _context.Wishlists
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.HotelId == hotelId);

                if (wishlist == null)
                    return false;

                _context.Wishlists.Remove(wishlist);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error removing wishlist item: {ex.Message}");
            }
        }
    }
}
