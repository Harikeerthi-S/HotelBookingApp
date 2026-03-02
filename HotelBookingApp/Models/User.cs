using System;
using System.Collections.Generic;

namespace HotelBookingApp.Models
{
    public class User : IComparable<User>, IEquatable<User>
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // dynamic role

        public byte[] Password { get; set; } = Array.Empty<byte>();
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public string? Phone { get; set; }


        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Wishlist>? Wishlists { get; set; }
        public ICollection<Notification>? Notifications { get; set; }

        public int CompareTo(User? other) => other != null ? UserId.CompareTo(other.UserId) : 1;
        public bool Equals(User? other) => other != null && UserId == other.UserId;
        public override bool Equals(object? obj) => Equals(obj as User);
        public override int GetHashCode() => UserId.GetHashCode();
        public override string ToString() => $"UserId: {UserId}, UserName: {UserName}, Email: {Email}, Role: {Role}";
    }

}