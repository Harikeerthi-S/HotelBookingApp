using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Interfaces.InterfaceServices
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> MakePaymentAsync(PaymentDto paymentDto);
        Task<PaymentResponseDto?> GetPaymentByIdAsync(int paymentId);
        Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync();
        Task<PaymentResponseDto?> UpdatePaymentStatusAsync(int paymentId, string newStatus);

    }
}
