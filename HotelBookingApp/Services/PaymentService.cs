using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using HotelBookingAppWebApi.Interfaces;

namespace HotelBookingApp.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<int, Booking> _bookingRepository;
        private readonly IRepository<int, Payment> _paymentRepository;

        public PaymentService(
            IRepository<int, Booking> bookingRepository,
            IRepository<int, Payment> paymentRepository)
        {
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
        }

        // ===============================
        // MAKE PAYMENT
        // ===============================
        public async Task<PaymentResponseDto> MakePaymentAsync(PaymentDto paymentDto)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(paymentDto.BookingId);
                if (booking == null) throw new Exception("Booking not found.");
                if (paymentDto.Amount <= 0) throw new Exception("Payment amount must be greater than zero.");
                if (string.IsNullOrWhiteSpace(paymentDto.PaymentMethod)) throw new Exception("Payment method is required.");

                string paymentStatus;

                if (paymentDto.PaymentMethod == "CreditCard" || paymentDto.PaymentMethod == "DebitCard")
                    paymentStatus = "Completed";
                else if (paymentDto.PaymentMethod == "UPI" || paymentDto.PaymentMethod == "Wallet" || paymentDto.PaymentMethod == "PayPal")
                    paymentStatus = "Pending";
                else
                    throw new Exception("Invalid payment method.");

                if (paymentDto.Amount < booking.TotalAmount)
                    paymentStatus = "Failed";

                var payment = new Payment
                {
                    BookingId = paymentDto.BookingId,
                    Amount = paymentDto.Amount,
                    PaymentMethod = paymentDto.PaymentMethod,
                    PaymentStatus = paymentStatus
                };

                var createdPayment = await _paymentRepository.AddAsync(payment);

                if (paymentStatus == "Completed") booking.Status = "Confirmed";
                else if (paymentStatus == "Failed") booking.Status = "Payment Failed";

                await _bookingRepository.UpdateAsync(booking.BookingId, booking);

                return new PaymentResponseDto
                {
                    PaymentId = createdPayment.PaymentId,
                    BookingId = createdPayment.BookingId,
                    Amount = createdPayment.Amount,
                    PaymentMethod = createdPayment.PaymentMethod,
                    PaymentStatus = createdPayment.PaymentStatus
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error making payment: {ex.Message}");
            }
        }

        // ===============================
        // UPDATE PAYMENT STATUS
        // ===============================
        public async Task<PaymentResponseDto?> UpdatePaymentStatusAsync(int paymentId, string newStatus)
        {
            try
            {
                var payment = await _paymentRepository.GetByIdAsync(paymentId);
                if (payment == null) return null;

                if (string.IsNullOrWhiteSpace(newStatus))
                    throw new Exception("Status cannot be empty.");

                if (newStatus != "Completed" && newStatus != "Failed" && newStatus != "Refunded" && newStatus != "Pending")
                    throw new Exception("Invalid payment status.");

                payment.PaymentStatus = newStatus;
                await _paymentRepository.UpdateAsync(paymentId, payment);

                var booking = await _bookingRepository.GetByIdAsync(payment.BookingId);
                if (booking != null)
                {
                    if (newStatus == "Completed") booking.Status = "Confirmed";
                    else if (newStatus == "Failed") booking.Status = "Payment Failed";
                    else if (newStatus == "Refunded") booking.Status = "Cancelled";

                    await _bookingRepository.UpdateAsync(booking.BookingId, booking);
                }

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
                var payment = await _paymentRepository.GetByIdAsync(paymentId);
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
                var payments = await _paymentRepository.GetAllAsync();
                return payments.Select(p => new PaymentResponseDto
                {
                    PaymentId = p.PaymentId,
                    BookingId = p.BookingId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving payments: {ex.Message}");
            }
        }

        // ===============================
        // GET PAGED PAYMENTS
        // ===============================
        public async Task<PagedResponseDto<PaymentResponseDto>> GetPaymentsPagedAsync(PagedRequestDto request)
        {
            try
            {
                if (request.PageNumber <= 0) request.PageNumber = 1;
                if (request.PageSize <= 0) request.PageSize = 10;

                var allPayments = await _paymentRepository.GetAllAsync();
                var totalRecords = allPayments.Count();

                var pagedPayments = allPayments
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(p => new PaymentResponseDto
                    {
                        PaymentId = p.PaymentId,
                        BookingId = p.BookingId,
                        Amount = p.Amount,
                        PaymentMethod = p.PaymentMethod,
                        PaymentStatus = p.PaymentStatus
                    })
                    .ToList();

                return new PagedResponseDto<PaymentResponseDto>
                {
                    Data = pagedPayments,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paged payments: {ex.Message}");
            }
        }
    }
}