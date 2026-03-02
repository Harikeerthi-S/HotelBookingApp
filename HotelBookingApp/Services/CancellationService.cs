using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Services
{
    public class CancellationService : ICancellationService
    {
        private readonly HotelBookingContext _context;

        public CancellationService(HotelBookingContext context)
        {
            _context = context;
        }

        public async Task<CancellationResponseDto> CreateCancellationAsync(CreateCancellationDto dto)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == dto.BookingId);
            if (booking == null) throw new InvalidOperationException("Booking not found.");
            if (booking.Status != "Confirmed") throw new InvalidOperationException($"Cannot cancel booking with status '{booking.Status}'.");

            decimal refundAmount = 0;
            if ((booking.CheckIn - DateTime.UtcNow).TotalHours >= 24)
                refundAmount = booking.TotalAmount * 0.8m;

            var cancellation = new Cancellation
            {
                BookingId = dto.BookingId,
                Reason = dto.Reason,
                RefundAmount = refundAmount,
                Status = "Pending",
                CancellationDate = DateTime.UtcNow
            };

            _context.Cancellations.Add(cancellation);
            booking.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return MapToResponseDto(cancellation);
        }

        public async Task<CancellationResponseDto?> GetCancellationByIdAsync(int cancellationId)
        {
            var cancellation = await _context.Cancellations.AsNoTracking().FirstOrDefaultAsync(c => c.CancellationId == cancellationId);
            return cancellation == null ? null : MapToResponseDto(cancellation);
        }

        public async Task<PagedResponseDto<CancellationResponseDto>> GetCancellationsByUserAsync(int userId, PagedRequestDto pageRequest)
        {
            if (pageRequest.PageNumber <= 0) pageRequest.PageNumber = 1;
            if (pageRequest.PageSize <= 0) pageRequest.PageSize = 10;

            var query = _context.Cancellations
                .Include(c => c.Booking)
                .Where(c => c.Booking!.UserId == userId)
                .AsNoTracking();

            var totalRecords = await query.CountAsync();

            var cancellations = await query
                .OrderByDescending(c => c.CancellationDate)
                .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize)
                .Select(c => MapToResponseDto(c))
                .ToListAsync();

            return new PagedResponseDto<CancellationResponseDto>
            {
                Data = cancellations,
                PageNumber = pageRequest.PageNumber,
                PageSize = pageRequest.PageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<CancellationResponseDto> UpdateCancellationStatusAsync(int cancellationId, string status, decimal refundAmount = 0)
        {
            var cancellation = await _context.Cancellations.Include(c => c.Booking).FirstOrDefaultAsync(c => c.CancellationId == cancellationId);
            if (cancellation == null) throw new InvalidOperationException("Cancellation not found.");

            var validStatuses = new[] { "Pending", "Approved", "Rejected", "Refunded" };
            if (!validStatuses.Contains(status)) throw new InvalidOperationException("Invalid cancellation status.");

            cancellation.Status = status;
            if (refundAmount > 0) cancellation.RefundAmount = refundAmount;
            if (status == "Refunded" && cancellation.Booking != null) cancellation.Booking.Status = "Refunded";

            await _context.SaveChangesAsync();

            return MapToResponseDto(cancellation);
        }

        private static CancellationResponseDto MapToResponseDto(Cancellation c)
        {
            return new CancellationResponseDto
            {
                CancellationId = c.CancellationId,
                BookingId = c.BookingId,
                Reason = c.Reason,
                RefundAmount = c.RefundAmount,
                Status = c.Status,
                CancellationDate = c.CancellationDate
            };
        }
    }
}