using HotelBookingApp.Models.Dtos;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingRoomController : ControllerBase
    {
        private readonly IBookingRoomService _service;

        public BookingRoomController(IBookingRoomService service)
        {
            _service = service;
        }

        // ==========================================
        // CREATE BOOKING ROOM
        // ==========================================
        [HttpPost]
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> Create([FromBody] CreateBookingRoomDto dto)
        {
            try
            {
                var result = await _service.CreateBookingRoomAsync(dto);
                return CreatedAtAction(nameof(GetById), new { bookingRoomId = result.BookingRoomId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error creating booking room", Details = ex.Message });
            }
        }

        // ==========================================
        // GET BOOKING ROOM BY ID
        // ==========================================
        [HttpGet("{bookingRoomId:int}")]
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> GetById(int bookingRoomId)
        {
            try
            {
                var result = await _service.GetBookingRoomByIdAsync(bookingRoomId);
                if (result == null) return NotFound(new { Message = "Booking room not found." });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving booking room", Details = ex.Message });
            }
        }

        // ==========================================
        // GET ALL BOOKING ROOMS FOR A BOOKING
        // ==========================================
        [HttpGet("booking/{bookingId:int}")]
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> GetByBookingId(int bookingId)
        {
            try
            {
                var list = await _service.GetBookingRoomsByBookingIdAsync(bookingId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving booking rooms", Details = ex.Message });
            }
        }

        // ==========================================
        // UPDATE BOOKING ROOM
        // ==========================================
        [HttpPut("{bookingRoomId:int}")]
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> Update(int bookingRoomId, [FromBody] CreateBookingRoomDto dto)
        {
            try
            {
                var result = await _service.UpdateBookingRoomAsync(bookingRoomId, dto);
                if (result == null) return NotFound(new { Message = "Booking room not found." });
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error updating booking room", Details = ex.Message });
            }
        }

        // ==========================================
        // DELETE BOOKING ROOM
        // ==========================================
        [HttpDelete("{bookingRoomId:int}")]
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> Delete(int bookingRoomId)
        {
            try
            {
                var success = await _service.DeleteBookingRoomAsync(bookingRoomId);
                if (!success) return NotFound(new { Message = "Booking room not found." });
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error deleting booking room", Details = ex.Message });
            }
        }
    }
}