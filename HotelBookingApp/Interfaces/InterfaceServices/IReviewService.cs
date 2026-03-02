using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Interfaces.InterfaceServices
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewResponseDto>> GetAllAsync();
        Task<ReviewResponseDto?> GetByIdAsync(int reviewId);
        Task<ReviewResponseDto> CreateAsync(CreateReviewDto dto);
        Task<bool> DeleteAsync(int reviewId);
    }
}
