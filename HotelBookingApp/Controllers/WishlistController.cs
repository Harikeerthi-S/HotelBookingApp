using HotelBookingApp.Models.Dtos;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _service;

        public WishlistController(IWishlistService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(WishlistDto dto)
        {
            try
            {
                var result = await _service.AddToWishlistAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserWishlist(int userId)
        {
            try
            {
                var result = await _service.GetUserWishlistAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{wishlistId}")]
        public async Task<IActionResult> Remove(int wishlistId)
        {
            try
            {
                var success = await _service.RemoveFromWishlistAsync(wishlistId);

                if (!success)
                    return NotFound("Wishlist item not found");

                return Ok("Removed successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveByUserAndHotel(int userId, int hotelId)
        {
            try
            {
                var success = await _service.RemoveByUserAndHotelAsync(userId, hotelId);

                if (!success)
                    return NotFound("Item not found");

                return Ok("Removed successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}