using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingApp.Models
{
    public class Wishlist : IComparable<Wishlist>, IEquatable<Wishlist>
    {
        public int WishlistId { get; set; }

        // Foreign keys
        public int UserId { get; set; }
        public int HotelId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [ForeignKey(nameof(HotelId))]
        public Hotel? Hotel { get; set; }

        // Comparisons and overrides
        public int CompareTo(Wishlist? other) => other != null ? WishlistId.CompareTo(other.WishlistId) : 1;

        public bool Equals(Wishlist? other) => other != null && WishlistId == other.WishlistId;

        public override bool Equals(object? obj) => Equals(obj as Wishlist);

        public override int GetHashCode() => WishlistId.GetHashCode();

        public override string ToString() => $"WishlistId: {WishlistId}, UserId: {UserId}, HotelId: {HotelId}";
    }
}