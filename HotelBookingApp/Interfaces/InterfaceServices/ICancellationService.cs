using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Services
{
    public interface ICancellationService
    {
        Task<CancellationResponseDto> CreateCancellationAsync(CreateCancellationDto dto);
        Task<CancellationResponseDto?> GetCancellationByIdAsync(int cancellationId);
        Task<PagedResponseDto<CancellationResponseDto>> GetCancellationsByUserAsync(int userId, PagedRequestDto pageRequest);
        Task<CancellationResponseDto> UpdateCancellationStatusAsync(int cancellationId, string status, decimal refundAmount = 0);
    }
}