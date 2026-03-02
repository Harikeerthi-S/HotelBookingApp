using HotelBookingApp.Models.Dtos;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelController(IHotelService hotelService)
        {
            _hotelService = hotelService ?? throw new ArgumentNullException(nameof(hotelService));
        }

        // ==========================================
        // ADMIN: Create a new hotel
        // ==========================================
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] CreateHotelDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { Message = "Hotel data is required." });

                var hotel = await _hotelService.CreateHotelAsync(dto);
                return CreatedAtAction(nameof(GetById), new { hotelId = hotel.HotelId }, hotel);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        // ==========================================
        // PUBLIC: Get paginated hotels
        // ==========================================
        [HttpGet("paged")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaged([FromQuery] PagedRequestDto request)
        {
            try
            {
                var result = await _hotelService.GetHotelsPagedAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to retrieve hotels.", Details = ex.Message });
            }
        }

        // ==========================================
        // PUBLIC: Filter hotels with pagination
        // ==========================================
        [HttpPost("filter")]
        [AllowAnonymous]
        public async Task<IActionResult> Filter([FromBody] HotelFilterDto filter, [FromQuery] PagedRequestDto request)
        {
            try
            {
                var result = await _hotelService.FilterHotelsPagedAsync(filter, request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to filter hotels.", Details = ex.Message });
            }
        }

        // ==========================================
        // PUBLIC: Get hotel by ID
        // ==========================================
        [HttpGet("{hotelId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int hotelId)
        {
            try
            {
                var hotel = await _hotelService.GetHotelByIdAsync(hotelId);
                if (hotel == null)
                    return NotFound(new { Message = "Hotel not found." });

                return Ok(hotel);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to retrieve hotel.", Details = ex.Message });
            }
        }

        // ==========================================
        // PUBLIC: Search hotels by location
        // ==========================================
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(location))
                    return BadRequest(new { Message = "Location query is required." });

                var hotels = await _hotelService.SearchHotelsAsync(location);
                return Ok(hotels);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to search hotels.", Details = ex.Message });
            }
        }

        // ==========================================
        // ADMIN: Update hotel
        // ==========================================
        [HttpPut("{hotelId:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int hotelId, [FromBody] CreateHotelDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { Message = "Hotel data is required." });

                var hotel = await _hotelService.UpdateHotelAsync(hotelId, dto);
                if (hotel == null)
                    return NotFound(new { Message = "Hotel not found." });

                return Ok(hotel);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to update hotel.", Details = ex.Message });
            }
        }

        // ==========================================
        // ADMIN: Soft delete hotel
        // ==========================================
        [HttpDelete("{hotelId:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Deactivate(int hotelId)
        {
            try
            {
                var result = await _hotelService.DeactivateHotelAsync(hotelId);
                if (!result)
                    return NotFound(new { Message = "Hotel not found." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to deactivate hotel.", Details = ex.Message });
            }
        }
    }
}