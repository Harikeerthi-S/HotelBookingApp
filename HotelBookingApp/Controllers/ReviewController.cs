using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // =========================================
        // GET PAGED + FILTERED REVIEWS (Admin, HotelManager)
        // =========================================
        [HttpPost("paged")]
        [Authorize(Roles = "admin,hotelmanager")]
        public async Task<IActionResult> GetReviewsPaged(
            [FromBody] ReviewFilterDto filter,
            [FromQuery] PagedRequestDto pagination)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsPagedAsync(filter, pagination);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // =========================================
        // GET REVIEW BY ID (All Roles)
        // =========================================
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,hotelmanager,user")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var review = await _reviewService.GetByIdAsync(id);
                if (review == null)
                    return NotFound(new { Message = "Review not found." });

                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // =========================================
        // CREATE REVIEW (User)
        // =========================================
        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
        {
            try
            {
                var created = await _reviewService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.ReviewId }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // =========================================
        // DELETE REVIEW (Admin, HotelManager)
        // =========================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,hotelmanager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _reviewService.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { Message = "Review not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}