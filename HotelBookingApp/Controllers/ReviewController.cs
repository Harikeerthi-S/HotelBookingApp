using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 🔐 All endpoints require authentication
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // ===============================
        // GET ALL REVIEWS
        // ===============================
        [HttpGet]
        [Authorize(Roles = "admin,user")] 
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var reviews = await _reviewService.GetAllAsync();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ===============================
        // GET REVIEW BY ID
        // ===============================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var review = await _reviewService.GetByIdAsync(id);

                if (review == null)
                    return NotFound("Review not found.");

                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ===============================
        // CREATE REVIEW
        // ===============================
        [HttpPost]
        [Authorize(Roles = "user")] // 🔐 Only User role can create review
        public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdReview = await _reviewService.CreateAsync(dto);

                return CreatedAtAction(nameof(GetById),
                    new { id = createdReview.ReviewId },
                    createdReview);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ===============================
        // DELETE REVIEW
        // ===============================
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")] // 🔐 Only Admin can delete reviews
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _reviewService.DeleteAsync(id);

                if (!deleted)
                    return NotFound("Review not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}