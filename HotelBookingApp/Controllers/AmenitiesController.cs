using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints require authentication unless marked AllowAnonymous
    public class AmenitiesController : ControllerBase
    {
        private readonly IAmenityService _amenityService;

        public AmenitiesController(IAmenityService amenityService)
        {
            _amenityService = amenityService;
        }

        // ======================================
        // GET ALL (Public)
        // ======================================
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var amenities = await _amenityService.GetAllAsync();
                return Ok(amenities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        // ======================================
        // GET BY ID (Public)
        // ======================================
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var amenity = await _amenityService.GetByIdAsync(id);

                if (amenity == null)
                    return NotFound("Amenity not found.");

                return Ok(amenity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        // ======================================
        // CREATE (Admin Only)
        // ======================================
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] CreateAmenityDto dto)
        {
            try
            {
                var created = await _amenityService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.AmenityId },
                    created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        // ======================================
        // UPDATE (Admin Only)
        // ======================================
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateAmenityDto dto)
        {
            try
            {
                var updated = await _amenityService.UpdateAsync(id, dto);

                if (!updated)
                    return NotFound("Amenity not found.");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        // ======================================
        // DELETE (Admin Only)
        // ======================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _amenityService.DeleteAsync(id);

                if (!deleted)
                    return NotFound("Amenity not found.");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // When amenity is assigned to hotels
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }
    }
}