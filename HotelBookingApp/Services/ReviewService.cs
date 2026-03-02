using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Services
{
    public class ReviewService : IReviewService
    {
        private readonly HotelBookingContext _context;

        public ReviewService(HotelBookingContext context)
        {
            _context = context;
        }

        // ===============================
        // GET ALL REVIEWS
        // ===============================
        public async Task<IEnumerable<ReviewResponseDto>> GetAllAsync()
        {
            try
            {
                var reviews = await _context.Reviews
                    .AsNoTracking()
                    .ToListAsync();

                return reviews.Select(r => new ReviewResponseDto
                {
                    ReviewId = r.ReviewId,
                    HotelId = r.HotelId,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Comment = r.Comment
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving reviews.", ex);
            }
        }

        // ===============================
        // GET REVIEW BY ID
        // ===============================
        public async Task<ReviewResponseDto?> GetByIdAsync(int reviewId)
        {
            try
            {
                var review = await _context.Reviews
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.ReviewId == reviewId);

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
        // CREATE REVIEW (BUSINESS LOGIC)
        // ===============================
        public async Task<ReviewResponseDto> CreateAsync(CreateReviewDto dto)
        {
            try
            {
                // 🔹 1. Validate Rating
                if (dto.Rating < 1 || dto.Rating > 5)
                    throw new ArgumentException("Rating must be between 1 and 5.");

                // 🔹 2. Check Hotel Exists
                var hotelExists = await _context.Hotels
                    .AnyAsync(h => h.HotelId == dto.HotelId);

                if (!hotelExists)
                    throw new KeyNotFoundException("Hotel not found.");

                // 🔹 3. Check User Exists
                var userExists = await _context.Users
                    .AnyAsync(u => u.UserId == dto.UserId);

                if (!userExists)
                    throw new KeyNotFoundException("User not found.");

                // 🔹 4. Prevent Duplicate Review
                var alreadyReviewed = await _context.Reviews
                    .AnyAsync(r => r.HotelId == dto.HotelId &&
                                   r.UserId == dto.UserId);

                if (alreadyReviewed)
                    throw new InvalidOperationException("User already reviewed this hotel.");

                var review = new Review
                {
                    HotelId = dto.HotelId,
                    UserId = dto.UserId,
                    Rating = dto.Rating,
                    Comment = dto.Comment
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

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
                var review = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.ReviewId == reviewId);

                if (review == null)
                    return false;

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting review with ID {reviewId}.", ex);
            }
        }
    }
}