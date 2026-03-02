using HotelBookingApp.Models.Dtos;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
        {
            try
            {
                var booking = await _bookingService.CreateBookingAsync(dto);
                return CreatedAtAction(nameof(GetById), new { bookingId = booking.BookingId }, booking);
            }
            catch (ArgumentException ex) { return BadRequest(new { Message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { Message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error creating booking", Details = ex.Message }); }
        }

        [HttpGet("{bookingId:int}")]
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> GetById(int bookingId)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(bookingId);
                if (booking == null) return NotFound(new { Message = "Booking not found." });
                return Ok(booking);
            }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error retrieving booking", Details = ex.Message }); }
        }

        [HttpGet("user/{userId:int}")]
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> GetByUser(int userId, [FromQuery] PagedRequestDto pageRequest)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByUserAsync(userId, pageRequest);
                return Ok(bookings);
            }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error retrieving user bookings", Details = ex.Message }); }
        }

        [HttpGet("hotel/{hotelId:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetByHotel(int hotelId, [FromQuery] PagedRequestDto pageRequest)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByHotelAsync(hotelId, pageRequest);
                return Ok(bookings);
            }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error retrieving hotel bookings", Details = ex.Message }); }
        }

        [HttpPost("{bookingId:int}/cancel")]
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> Cancel(int bookingId)
        {
            try
            {
                var result = await _bookingService.CancelBookingAsync(bookingId);
                if (!result) return NotFound(new { Message = "Booking not found." });
                return Ok(new { Message = "Booking cancelled successfully." });
            }
            catch (InvalidOperationException ex) { return BadRequest(new { Message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error cancelling booking", Details = ex.Message }); }
        }

        [HttpPost("{bookingId:int}/complete")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Complete(int bookingId)
        {
            try
            {
                var booking = await _bookingService.CompleteBookingAsync(bookingId);
                return Ok(booking);
            }
            catch (InvalidOperationException ex) { return BadRequest(new { Message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error completing booking", Details = ex.Message }); }
        }
    }
}