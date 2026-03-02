using HotelBookingApp.Models.Dtos;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancellationController : ControllerBase
    {
        private readonly ICancellationService _cancellationService;

        public CancellationController(ICancellationService cancellationService)
        {
            _cancellationService = cancellationService;
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Create([FromBody] CreateCancellationDto dto)
        {
            try
            {
                var cancellation = await _cancellationService.CreateCancellationAsync(dto);
                return CreatedAtAction(nameof(GetById), new { cancellationId = cancellation.CancellationId }, cancellation);
            }
            catch (InvalidOperationException ex) { return BadRequest(new { Message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error creating cancellation", Details = ex.Message }); }
        }

        [HttpGet("{cancellationId:int}")]
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> GetById(int cancellationId)
        {
            try
            {
                var cancellation = await _cancellationService.GetCancellationByIdAsync(cancellationId);
                if (cancellation == null) return NotFound(new { Message = "Cancellation not found." });
                return Ok(cancellation);
            }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error retrieving cancellation", Details = ex.Message }); }
        }

        [HttpGet("user/{userId:int}")]
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> GetByUser(int userId, [FromQuery] PagedRequestDto pageRequest)
        {
            try
            {
                var cancellations = await _cancellationService.GetCancellationsByUserAsync(userId, pageRequest);
                return Ok(cancellations);
            }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error retrieving cancellations", Details = ex.Message }); }
        }

        [HttpPut("{cancellationId:int}/status")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateStatus(int cancellationId, [FromQuery] string status, [FromQuery] decimal refundAmount = 0)
        {
            try
            {
                var updated = await _cancellationService.UpdateCancellationStatusAsync(cancellationId, status, refundAmount);
                return Ok(updated);
            }
            catch (InvalidOperationException ex) { return BadRequest(new { Message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error updating cancellation status", Details = ex.Message }); }
        }
    }
}