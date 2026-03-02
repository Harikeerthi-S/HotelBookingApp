namespace HotelBookingApp.Models
{
    public class HotelAmenity : IComparable<HotelAmenity>, IEquatable<HotelAmenity>
    {
        public int HotelAmenityId { get; set; }

        public int HotelId { get; set; }

        public int AmenityId { get; set; }

        public Hotel? Hotel { get; set; }

        public Amenity? Amenity { get; set; }

        public int CompareTo(HotelAmenity? other)
        {
            return other != null ? HotelAmenityId.CompareTo(other.HotelAmenityId) : 1;
        }

        public bool Equals(HotelAmenity? other)
        {
            return other != null && HotelAmenityId == other.HotelAmenityId;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as HotelAmenity);
        }

        public override int GetHashCode()
        {
            return HotelAmenityId.GetHashCode();
        }

        public override string ToString()
        {
            return $"HotelAmenityId: {HotelAmenityId}, HotelId: {HotelId}, AmenityId: {AmenityId}";
        }
    }
}
