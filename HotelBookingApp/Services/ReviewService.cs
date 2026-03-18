using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using HotelBookingAppWebApi.Interfaces;
using System.Linq.Expressions;

namespace HotelBookingApp.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository<int, Review> _reviewRepository;
        private readonly IRepository<int, Hotel> _hotelRepository;
        private readonly IRepository<int, User> _userRepository;

        public ReviewService(
            IRepository<int, Review> reviewRepository,
            IRepository<int, Hotel> hotelRepository,
            IRepository<int, User> userRepository)
        {
            _reviewRepository = reviewRepository;
            _hotelRepository = hotelRepository;
            _userRepository = userRepository;
        }

        // ===============================
        // GET REVIEWS WITH FILTER + PAGINATION
        // ===============================
        public async Task<PagedResponseDto<ReviewResponseDto>> GetReviewsPagedAsync(
            ReviewFilterDto filter,
            PagedRequestDto pageRequest)
        {
            try
            {
                if (pageRequest.PageNumber <= 0)
                    pageRequest.PageNumber = 1;

                if (pageRequest.PageSize <= 0)
                    pageRequest.PageSize = 10;

                // Build filter expression
                Expression<Func<Review, bool>> predicate = r => true;

                if (filter.HotelId.HasValue)
                    predicate = r => r.HotelId == filter.HotelId.Value;

                if (filter.UserId.HasValue)
                    predicate = r => r.UserId == filter.UserId.Value;

                if (filter.Rating.HasValue)
                    predicate = r => r.Rating == filter.Rating.Value;

                // Get filtered data
                var allReviews = await _reviewRepository.GetAllAsync();
                var query = allReviews.AsQueryable().Where(predicate);

                var totalRecords = query.Count();

                var data = query
                    .OrderBy(r => r.ReviewId)
                    .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                    .Take(pageRequest.PageSize)
                    .Select(r => new ReviewResponseDto
                    {
                        ReviewId = r.ReviewId,
                        HotelId = r.HotelId,
                        UserId = r.UserId,
                        Rating = r.Rating,
                        Comment = r.Comment
                    })
                    .ToList();

                return new PagedResponseDto<ReviewResponseDto>
                {
                    Data = data,
                    PageNumber = pageRequest.PageNumber,
                    PageSize = pageRequest.PageSize,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving paged reviews.", ex);
            }
        }

        // ===============================
        // GET REVIEW BY ID
        // ===============================
        public async Task<ReviewResponseDto?> GetByIdAsync(int reviewId)
        {
            try
            {
                var review = await _reviewRepository.GetByIdAsync(reviewId);

                if (review == null)
                    return null;

                return new ReviewResponseDto
                {
                    ReviewId = review.ReviewId,
                    HotelId = review.HotelId,
                    UserId = review.UserId,
                    Rating = review.Rating,
                    Comment = review.Comment
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving review with ID {reviewId}.", ex);
            }
        }

        // ===============================
        // CREATE REVIEW
        // ===============================
        public async Task<ReviewResponseDto> CreateAsync(CreateReviewDto dto)
        {
            try
            {
                if (dto.Rating < 1 || dto.Rating > 5)
                    throw new ArgumentException("Rating must be between 1 and 5.");

                var hotel = await _hotelRepository.GetByIdAsync(dto.HotelId);
                if (hotel == null)
                    throw new KeyNotFoundException("Hotel not found.");

                var user = await _userRepository.GetByIdAsync(dto.UserId);
                if (user == null)
                    throw new KeyNotFoundException("User not found.");

                var reviews = await _reviewRepository.GetAllAsync();
                if (reviews.Any(r => r.HotelId == dto.HotelId && r.UserId == dto.UserId))
                    throw new InvalidOperationException("User already reviewed this hotel.");

                var review = new Review
                {
                    HotelId = dto.HotelId,
                    UserId = dto.UserId,
                    Rating = dto.Rating,
                    Comment = dto.Comment
                };

                var created = await _reviewRepository.AddAsync(review);

                return new ReviewResponseDto
                {
                    ReviewId = created.ReviewId,
                    HotelId = created.HotelId,
                    UserId = created.UserId,
                    Rating = created.Rating,
                    Comment = created.Comment
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating review.", ex);
            }
        }

        // ===============================
        // DELETE REVIEW
        // ===============================
        public async Task<bool> DeleteAsync(int reviewId)
        {
            try
            {
                var deleted = await _reviewRepository.DeleteAsync(reviewId);
                return deleted != null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting review with ID {reviewId}.", ex);
            }
        }
    }
}