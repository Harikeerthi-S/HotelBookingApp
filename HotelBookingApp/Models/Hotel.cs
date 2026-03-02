namespace HotelBookingApp.Models
{
    public class Hotel : IComparable<Hotel>, IEquatable<Hotel>
    {
        public int HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double StarRating { get; set; }

        public bool IsActive { get; set; } = true;

        public string ContactNumber { get; set; } = string.Empty;
        public ICollection<Room>? Rooms { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Wishlist>? Wishlists { get; set; }
        public ICollection<HotelAmenity>? HotelAmenities { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public int CompareTo(Hotel? other)
        {
            return other != null ? HotelId.CompareTo(other.HotelId) : 1;
        }

        public bool Equals(Hotel? other)
        {
            return other != null && HotelId == other.HotelId;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Hotel);
        }

        public override int GetHashCode()
        {
            return HotelId.GetHashCode();
        }

        public override string ToString()
        {
            return $"HotelId: {HotelId}, HotelName: {HotelName}, Location: {Location}, Rating: {StarRating}";
        }
    }
}
 