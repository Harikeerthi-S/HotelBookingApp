namespace HotelBookingApp.Models
{
    public class Notification : IComparable<Notification>, IEquatable<Notification>
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User? User { get; set; }
        public int CompareTo(Notification? other)
        {
            return other != null ? NotificationId.CompareTo(other.NotificationId) : 1;
        }

        public bool Equals(Notification? other)
        {
            return other != null && NotificationId == other.NotificationId;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Notification);
        }

        public override int GetHashCode()
        {
            return NotificationId.GetHashCode();
        }

        public override string ToString()
        {
            return $"NotificationId: {NotificationId}, Message: {Message}, IsRead: {IsRead}";
        }
    }
}
