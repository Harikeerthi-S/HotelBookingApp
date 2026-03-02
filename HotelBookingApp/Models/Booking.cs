namespace HotelBookingApp.Models
{
    public class Booking : IComparable<Booking>, IEquatable<Booking>
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int HotelId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();

        public ICollection<Cancellation>? Cancellations { get; set; }
        public Room? Room { get; set; }
        public User? User { get; set; }
        public Hotel? Hotel { get; set; }

        public int CompareTo(Booking? other)
        {
            return other != null ? BookingId.CompareTo(other.BookingId) : 1;
        }

        public bool Equals(Booking? other)
        {
            return other != null && BookingId == other.BookingId;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Booking);
        }

        public override int GetHashCode()
        {
            return BookingId.GetHashCode();
        }

        public override string ToString()
        {
            return $"BookingId: {BookingId}, UserId: {UserId}, HotelId: {HotelId}, TotalAmount: {TotalAmount}";
        }
    }
}
