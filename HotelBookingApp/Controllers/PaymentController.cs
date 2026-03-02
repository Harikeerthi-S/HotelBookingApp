using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models.Dtos;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints require authentication
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // =====================================
        // MAKE PAYMENT (Only User Role)
        // =====================================
        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> MakePayment([FromBody] PaymentDto paymentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _paymentService.MakePaymentAsync(paymentDto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Payment failed.",
                    error = ex.Message
                });
            }
        }

        // =====================================
        // GET ALL PAYMENTS (Admin Only)
        // =====================================
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllPayments()
        {
            try
            {
                var payments = await _paymentService.GetAllPaymentsAsync();
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error retrieving payments.",
                    error = ex.Message
                });
            }
        }

        // =====================================
        // GET PAYMENT BY ID (Admin or User)
        // =====================================
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByIdAsync(id);

                if (payment == null)
                    return NotFound(new { message = "Payment not found." });

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error retrieving payment.",
                    error = ex.Message
                });
            }
        }

        // =====================================
        // UPDATE PAYMENT STATUS (Admin Only)
        // =====================================
        [HttpPut("{id}/status")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromQuery] string status)
        {
            try
            {
                var updatedPayment = await _paymentService.UpdatePaymentStatusAsync(id, status);

                if (updatedPayment == null)
                    return NotFound(new { message = "Payment not found." });

                return Ok(updatedPayment);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Failed to update payment status.",
                    error = ex.Message
                });
            }
        }
    }
}
