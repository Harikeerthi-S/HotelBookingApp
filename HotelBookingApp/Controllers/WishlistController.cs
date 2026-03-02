using HotelBookingApp.Models.Dtos;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        // ==========================================
        // ADD TO WISHLIST (User Only)
        // ==========================================
        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddToWishlist(WishlistDto dto)
        {
            try
            {
                var userIdFromToken = int.Parse(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                // Ensure user can only add for themselves
                if (dto.UserId != userIdFromToken)
                    return Forbid();

                var result = await _wishlistService.AddToWishlistAsync(dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Failed to add to wishlist.",
                    error = ex.Message
                });
            }
        }

        // ==========================================
        // GET MY WISHLIST (User)
        // ==========================================
        [HttpGet("my")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetMyWishlist()
        {
            try
            {
                var userId = int.Parse(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var wishlist = await _wishlistService
                    .GetUserWishlistAsync(userId);

                return Ok(wishlist);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error retrieving wishlist.",
                    error = ex.Message
                });
            }
        }

        // ==========================================
        // GET USER WISHLIST (Admin Only)
        // ==========================================
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUserWishlist(int userId)
        {
            try
            {
                var wishlist = await _wishlistService
                    .GetUserWishlistAsync(userId);

                return Ok(wishlist);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error retrieving wishlist.",
                    error = ex.Message
                });
            }
        }

        // ==========================================
        // REMOVE BY WISHLIST ID
        // ==========================================
        [HttpDelete("{wishlistId}")]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> RemoveFromWishlist(int wishlistId)
        {
            try
            {
                var result = await _wishlistService
                    .RemoveFromWishlistAsync(wishlistId);

                if (!result)
                    return NotFound(new { message = "Wishlist item not found." });

                return Ok(new { message = "Removed from wishlist successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Failed to remove wishlist item.",
                    error = ex.Message
                });
            }
        }

        // ==========================================
        // REMOVE BY USER + HOTEL (User Only)
        // ==========================================
        [HttpDelete("remove")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> RemoveByUserAndHotel([FromQuery] int hotelId)
        {
            try
            {
                var userId = int.Parse(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var result = await _wishlistService
                    .RemoveByUserAndHotelAsync(userId, hotelId);

                if (!result)
                    return NotFound(new { message = "Wishlist item not found." });

                return Ok(new { message = "Removed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Failed to remove wishlist item.",
                    error = ex.Message
                });
            }
        }
    }
}
