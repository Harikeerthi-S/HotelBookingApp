namespace HotelBookingApp.Models
{
    public class Room : IComparable<Room>, IEquatable<Room>
    {
        public int RoomId { get; set; }
        public int HotelId { get; set; }
        public int RoomNumber {  get; set; }
        public string RoomType { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int Capacity { get; set; }

        public Hotel? Hotel { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<BookingRoom>? BookingRooms { get; set; }
        public int CompareTo(Room? other)
        {
            return other != null ? RoomId.CompareTo(other.RoomId) : 1;
        }

        public bool Equals(Room? other)
        {
            return other != null && RoomId == other.RoomId;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Room);
        }

        public override int GetHashCode()
        {
            return RoomId.GetHashCode();
        }

        public override string ToString()
        {
            return $"RoomId: {RoomId}, RoomType: {RoomType}, Price: {PricePerNight}, Capacity: {Capacity}";
        }
    }
}