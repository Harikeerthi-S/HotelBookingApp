using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HotelBookingContext _context;

        public PaymentService(HotelBookingContext context)
        {
            _context = context;
        }

        // ===============================
        // MAKE PAYMENT
        // ===============================
        public async Task<PaymentResponseDto> MakePaymentAsync(PaymentDto paymentDto)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(paymentDto.BookingId);

                if (booking == null)
                    throw new Exception("Booking not found.");

                if (paymentDto.Amount <= 0)
                    throw new Exception("Payment amount must be greater than zero.");

                if (string.IsNullOrWhiteSpace(paymentDto.PaymentMethod))
                    throw new Exception("Payment method is required.");

                string paymentStatus;

                // Business logic for payment method
                if (paymentDto.PaymentMethod == "CreditCard" ||
                    paymentDto.PaymentMethod == "DebitCard")
                {
                    // Assume card payments are processed instantly
                    paymentStatus = "Completed";
                }
                else if (paymentDto.PaymentMethod == "UPI" ||
                         paymentDto.PaymentMethod == "Wallet")
                {
                    paymentStatus = "Pending";
                }
                else if (paymentDto.PaymentMethod == "PayPal")
                {
                    paymentStatus = "Pending";
                }
                else
                {
                    throw new Exception("Invalid payment method.");
                }

                // Additional Business Logic:
                // If payment amount is less than booking total → Failed
                if (paymentDto.Amount < booking.TotalAmount)
                {
                    paymentStatus = "Failed";
                }

                var payment = new Payment
                {
                    BookingId = paymentDto.BookingId,
                    Amount = paymentDto.Amount,
                    PaymentMethod = paymentDto.PaymentMethod,
                    PaymentStatus = paymentStatus
                };

                _context.Payments.Add(payment);

                // If payment completed → update booking status
                if (paymentStatus == "Completed")
                {
                    booking.Status = "Confirmed";
                }
                else if (paymentStatus == "Failed")
                {
                    booking.Status = "Payment Failed";
                }

                await _context.SaveChangesAsync();

                return new PaymentResponseDto
                {
                    PaymentId = payment.PaymentId,
                    BookingId = payment.BookingId,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    PaymentStatus = payment.PaymentStatus
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error processing payment: {ex.Message}");
            }
        }

        // ===============================
        // UPDATE PAYMENT STATUS
        // ===============================
        public async Task<PaymentResponseDto?> UpdatePaymentStatusAsync(int paymentId, string newStatus)
        {
            try
            {
                var payment = await _context.Payments
                    .Include(p => p.Booking)
                    .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

                if (payment == null)
                    return null;

                if (string.IsNullOrWhiteSpace(newStatus))
                    throw new Exception("Status cannot be empty.");

                // Allowed statuses
                if (newStatus != "Completed" &&
                    newStatus != "Failed" &&
                    newStatus != "Refunded" &&
                    newStatus != "Pending")
                {
                    throw new Exception("Invalid payment status.");
                }

                payment.PaymentStatus = newStatus;

                // Update booking based on payment status
                if (newStatus == "Completed")
                {
                    payment.Booking!.Status = "Confirmed";
                }
                else if (newStatus == "Failed")
                {
                    payment.Booking!.Status = "Payment Failed";
                }
                else if (newStatus == "Refunded")
                {
                    payment.Booking!.Status = "Cancelled";
                }

                await _context.SaveChangesAsync();

                return new PaymentResponseDto
                {
                    PaymentId = payment.PaymentId,
                    BookingId = payment.BookingId,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    PaymentStatus = payment.PaymentStatus
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating payment status: {ex.Message}");
            }
        }

        // ===============================
        // GET PAYMENT BY ID
        // ===============================
        public async Task<PaymentResponseDto?> GetPaymentByIdAsync(int paymentId)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(paymentId);
                if (payment == null) return null;

                return new PaymentResponseDto
                {
                    PaymentId = payment.PaymentId,
                    BookingId = payment.BookingId,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    PaymentStatus = payment.PaymentStatus
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving payment: {ex.Message}");
            }
        }

        // ===============================
        // GET ALL PAYMENTS
        // ===============================
        public async Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync()
        {
            try
            {
                return await _context.Payments
                    .Select(p => new PaymentResponseDto
                    {
                        PaymentId = p.PaymentId,
                        BookingId = p.BookingId,
                        Amount = p.Amount,
                        PaymentMethod = p.PaymentMethod,
                        PaymentStatus = p.PaymentStatus
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving payments: {ex.Message}");
            }
        }
    }
}
