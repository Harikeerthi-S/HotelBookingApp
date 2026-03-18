using HotelBookingApp.Models.Dtos;

namespace HotelBookingApp.Interfaces.InterfaceServices
{
    public interface IReviewService
    {
        Task<PagedResponseDto<ReviewResponseDto>> GetReviewsPagedAsync(
            ReviewFilterDto filter,
            PagedRequestDto pageRequest);

        Task<ReviewResponseDto?> GetByIdAsync(int reviewId);

        Task<ReviewResponseDto> CreateAsync(CreateReviewDto dto);

        Task<bool> DeleteAsync(int reviewId);
    }
}