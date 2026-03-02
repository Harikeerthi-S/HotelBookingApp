namespace HotelBookingApp.Models.Dtos
{
    public class RoomFilterDto
    {
        public int? HotelId { get; set; }
        public string? RoomType { get; set; } // e.g., "Single", "Double"
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinCapacity { get; set; }
        public int? MaxCapacity { get; set; }
        public bool OnlyAvailable { get; set; } = true;
    }
}
