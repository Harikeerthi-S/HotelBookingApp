using HotelBookingApp.Models.Dtos;
using HotelBookingApp.Interfaces.InterfaceServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HotelBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints require authentication
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        // ============================================
        // CREATE BOOKING
        // ============================================
        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            try
            {
                var booking = await _bookingService.CreateBookingAsync(dto);
                return CreatedAtAction(nameof(GetBookingById), new { bookingId = booking.BookingId }, booking);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        // ============================================
        // GET BOOKING BY ID
        // ============================================
        [HttpGet("{bookingId:int}")]
        [Authorize(Roles = "user,admin,hotelmanager")]
        public async Task<IActionResult> GetBookingById(int bookingId)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(bookingId);
                if (booking == null) return NotFound(new { Message = "Booking not found." });
                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving booking", Details = ex.Message });
            }
        }
        // ============================================
        // GET BOOKINGS BY USER WITH POST PAGINATION
        // ============================================
        [HttpPost("user/{userId:int}/paged")]
        [Authorize(Roles = "user,admin,hotelmanager")]
        public async Task<IActionResult> GetBookingsByUserPaged(int userId, [FromBody] PagedRequestDto pageRequest)
        {
            try
            {
                var pagedBookings = await _bookingService.GetBookingsByUserAsync(userId, pageRequest);
                return Ok(pagedBookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving bookings", Details = ex.Message });
            }
        }
        // ============================================
        // GET BOOKINGS BY HOTEL WITH POST PAGINATION
        // ============================================
        [HttpPost("hotel/{hotelId:int}/paged")]
        [Authorize(Roles = "admin,hotelmanager")]
        public async Task<IActionResult> GetBookingsByHotelPaged(int hotelId, [FromBody] PagedRequestDto pageRequest)
        {
            try
            {
                var pagedBookings = await _bookingService.GetBookingsByHotelAsync(hotelId, pageRequest);
                return Ok(pagedBookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving bookings", Details = ex.Message });
            }
        }
        // ============================================
        // CONFIRM BOOKING (Admin/HotelManager)
        // ============================================
        [HttpPut("{bookingId:int}/confirm")]
        [Authorize(Roles = "admin,hotelmanager")]
        public async Task<IActionResult> ConfirmBooking(int bookingId)
        {
            try
            {
                var booking = await _bookingService.ConfirmBookingAsync(bookingId);
                return Ok(booking);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        // ============================================
        // CANCEL BOOKING (User/Admin)
        // ============================================
        [HttpPut("{bookingId:int}/cancel")]
        [Authorize(Roles = "user,admin,hotelmanager")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            try
            {
                var result = await _bookingService.CancelBookingAsync(bookingId);
                if (!result) return NotFound(new { Message = "Booking not found." });
                return Ok(new { Message = "Booking cancelled successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        // ============================================
        // COMPLETE BOOKING (Admin/HotelManager)
        // ============================================
        [HttpPut("{bookingId:int}/complete")]
        [Authorize(Roles = "admin,hotelmanager")]
        public async Task<IActionResult> CompleteBooking(int bookingId)
        {
            try
            {
                var booking = await _bookingService.CompleteBookingAsync(bookingId);
                return Ok(booking);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
