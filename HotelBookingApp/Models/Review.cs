namespace HotelBookingApp.Models
{
    public class Review : IComparable<Review>, IEquatable<Review>
    {
        public int ReviewId { get; set; }
        public int HotelId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public User? User { get; set; }

        public Hotel? Hotel { get; set; }
        public int CompareTo(Review? other)
        {
            return other != null ? ReviewId.CompareTo(other.ReviewId) : 1;
        }

        public bool Equals(Review? other)
        {
            return other != null && ReviewId == other.ReviewId;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Review);
        }

        public override int GetHashCode()
        {
            return ReviewId.GetHashCode();
        }

        public override string ToString()
        {
            return $"ReviewId: {ReviewId}, Rating: {Rating}, Comment: {Comment}";
        }
    }
}
