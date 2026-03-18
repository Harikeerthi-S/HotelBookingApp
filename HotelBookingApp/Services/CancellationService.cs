using HotelBookingApp.Interfaces.InterfaceServices;
using HotelBookingApp.Models;
using HotelBookingApp.Models.Dtos;
using HotelBookingAppWebApi.Interfaces;

namespace HotelBookingApp.Services
{
    public class CancellationService : ICancellationService
    {
        private readonly IRepository<int, Booking> _bookingRepository;
        private readonly IRepository<int, Cancellation> _cancellationRepository;

        public CancellationService(
            IRepository<int, Booking> bookingRepository,
            IRepository<int, Cancellation> cancellationRepository)
        {
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
            _cancellationRepository = cancellationRepository ?? throw new ArgumentNullException(nameof(cancellationRepository));
        }

        // ===============================
        // CREATE CANCELLATION
        // ===============================
        public async Task<CancellationResponseDto> CreateCancellationAsync(CreateCancellationDto dto)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(dto.BookingId)
                              ?? throw new InvalidOperationException("Booking not found.");

                if (booking.Status != "Confirmed")
                    throw new InvalidOperationException($"Cannot cancel booking with status '{booking.Status}'.");

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

                await _cancellationRepository.AddAsync(cancellation);

                // Update booking status
                booking.Status = "Cancelled";
                await _bookingRepository.UpdateAsync(booking.BookingId, booking);

                return MapToResponseDto(cancellation);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating cancellation.", ex);
            }
        }

        // ===============================
        // GET CANCELLATION BY ID
        // ===============================
        public async Task<CancellationResponseDto?> GetCancellationByIdAsync(int cancellationId)
        {
            try
            {
                var cancellation = await _cancellationRepository.GetByIdAsync(cancellationId);
                return cancellation == null ? null : MapToResponseDto(cancellation);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving cancellation with ID {cancellationId}.", ex);
            }
        }

        // ===============================
        // GET CANCELLATIONS BY USER (PAGED)
        // ===============================
        public async Task<PagedResponseDto<CancellationResponseDto>> GetCancellationsByUserAsync(int userId, PagedRequestDto pageRequest)
        {
            try
            {
                if (pageRequest.PageNumber <= 0) pageRequest.PageNumber = 1;
                if (pageRequest.PageSize <= 0) pageRequest.PageSize = 10;

                var allCancellations = (await _cancellationRepository.GetAllAsync())
                                       .Where(c => c.Booking != null && c.Booking.UserId == userId)
                                       .OrderByDescending(c => c.CancellationDate)
                                       .ToList();

                var totalRecords = allCancellations.Count;

                var pagedCancellations = allCancellations
                    .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                    .Take(pageRequest.PageSize)
                    .Select(MapToResponseDto)
                    .ToList();

                return new PagedResponseDto<CancellationResponseDto>
                {
                    Data = pagedCancellations,
                    PageNumber = pageRequest.PageNumber,
                    PageSize = pageRequest.PageSize,
                    TotalRecords = totalRecords
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving cancellations for user.", ex);
            }
        }

        // ===============================
        // UPDATE CANCELLATION STATUS
        // ===============================
        public async Task<CancellationResponseDto> UpdateCancellationStatusAsync(int cancellationId, string status, decimal refundAmount = 0)
        {
            try
            {
                var cancellation = await _cancellationRepository.GetByIdAsync(cancellationId)
                                   ?? throw new InvalidOperationException("Cancellation not found.");

                var booking = cancellation.Booking;

                var validStatuses = new[] { "Pending", "Approved", "Rejected", "Refunded" };
                if (!validStatuses.Contains(status))
                    throw new InvalidOperationException("Invalid cancellation status.");

                cancellation.Status = status;
                if (refundAmount > 0) cancellation.RefundAmount = refundAmount;

                if (status == "Refunded" && booking != null)
                {
                    booking.Status = "Refunded";
                    await _bookingRepository.UpdateAsync(booking.BookingId, booking);
                }

                await _cancellationRepository.UpdateAsync(cancellation.CancellationId, cancellation);

                return MapToResponseDto(cancellation);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating cancellation status.", ex);
            }
        }

        // ===============================
        // PRIVATE MAPPING
        // ===============================
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