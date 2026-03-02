using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Services
{
    public interface IWishlistService
    {
        Task<WishlistResponseDto> AddToWishlistAsync(WishlistDto dto);
        Task<IEnumerable<WishlistResponseDto>> GetUserWishlistAsync(int userId);
        Task<bool> RemoveFromWishlistAsync(int wishlistId);
        Task<bool> RemoveByUserAndHotelAsync(int userId, int hotelId);
    }
}
