using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Interfaces.InterfaceServices
{
    public interface IPaymentService
    {
        // Make a new payment
        Task<PaymentResponseDto> MakePaymentAsync(PaymentDto paymentDto);

        // Update the status of an existing payment
        Task<PaymentResponseDto?> UpdatePaymentStatusAsync(int paymentId, string newStatus);

        // Get a payment by its ID
        Task<PaymentResponseDto?> GetPaymentByIdAsync(int paymentId);

        // Get all payments (non-paginated)
        Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync();

        // Get payments with pagination
        Task<PagedResponseDto<PaymentResponseDto>> GetPaymentsPagedAsync(PagedRequestDto request);
    }
}