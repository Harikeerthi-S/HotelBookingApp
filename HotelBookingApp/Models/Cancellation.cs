namespace HotelBookingApp.Models
{
    public class Cancellation : IComparable<Cancellation>, IEquatable<Cancellation>
    {
        public int CancellationId { get; set; }

        public int BookingId { get; set; }

        // Reason given by user
        public string Reason { get; set; } = string.Empty;

        // Refund amount after cancellation policy
        public decimal RefundAmount { get; set; }

        // Status: Pending / Approved / Rejected / Refunded
        public string Status { get; set; } = string.Empty;

        public DateTime CancellationDate { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public Booking? Booking { get; set; }

        public int CompareTo(Cancellation? other)
        {
            return other != null ? CancellationId.CompareTo(other.CancellationId) : 1;
        }

        public bool Equals(Cancellation? other)
        {
            return other != null && CancellationId == other.CancellationId;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Cancellation);
        }

        public override int GetHashCode()
        {
            return CancellationId.GetHashCode();
        }

        public override string ToString()
        {
            return $"CancellationId: {CancellationId}, BookingId: {BookingId}, RefundAmount: {RefundAmount}, Status: {Status}, Date: {CancellationDate}";
        }
    }
}
