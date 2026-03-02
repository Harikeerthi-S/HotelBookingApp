namespace HotelBookingApp.Models
{
    public class Payment : IComparable<Payment>, IEquatable<Payment>
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;

        public Booking? Booking { get; set; }

        public int CompareTo(Payment? other)
        {
            return other != null ? PaymentId.CompareTo(other.PaymentId) : 1;
        }

        public bool Equals(Payment? other)
        {
            return other != null && PaymentId == other.PaymentId;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Payment);
        }

        public override int GetHashCode()
        {
            return PaymentId.GetHashCode();
        }

        public override string ToString()
        {
            return $"PaymentId: {PaymentId}, BookingId: {BookingId}, Amount: {Amount}, Status: {PaymentStatus}";
        }
    }
}
