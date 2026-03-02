namespace HotelBookingApp.Models
{
    public class BookingRoom : IComparable<BookingRoom>, IEquatable<BookingRoom>
    {
        public int BookingRoomId { get; set; }

        public int BookingId { get; set; }

        public int RoomId { get; set; }

        // Price of this room at time of booking
        public decimal PricePerNight { get; set; }

        public int NumberOfRooms { get; set; }

        // Navigation Properties
        public Booking? Booking { get; set; }

        public Room? Room { get; set; }

        public int CompareTo(BookingRoom? other)
        {
            return other != null ? BookingRoomId.CompareTo(other.BookingRoomId) : 1;
        }

        public bool Equals(BookingRoom? other)
        {
            return other != null && BookingRoomId == other.BookingRoomId;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BookingRoom);
        }

        public override int GetHashCode()
        {
            return BookingRoomId.GetHashCode();
        }

        public override string ToString()
        {
            return $"BookingRoomId: {BookingRoomId}, BookingId: {BookingId}, RoomId: {RoomId}, PricePerNight: {PricePerNight}, NumberOfRooms: {NumberOfRooms}";
        }
    }
}
