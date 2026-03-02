namespace HotelBookingApp.Models
{
    public class Amenity : IComparable<Amenity>, IEquatable<Amenity>
    {
        public int AmenityId { get; set; }

        public string Name { get; set; } = string.Empty;   // WiFi, Pool, Parking

        public string? Description { get; set; }

        public string? Icon { get; set; }   // optional icon name or URL

        public ICollection<HotelAmenity>? HotelAmenities { get; set; }

        public int CompareTo(Amenity? other)
        {
            return other != null ? AmenityId.CompareTo(other.AmenityId) : 1;
        }

        public bool Equals(Amenity? other)
        {
            return other != null && AmenityId == other.AmenityId;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Amenity);
        }

        public override int GetHashCode()
        {
            return AmenityId.GetHashCode();
        }

        public override string ToString()
        {
            return $"AmenityId: {AmenityId}, Name: {Name}, Description: {Description}, Icon: {Icon}";
        }
    }
}
