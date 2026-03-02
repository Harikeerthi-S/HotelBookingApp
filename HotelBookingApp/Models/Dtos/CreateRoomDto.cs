namespace HotelBookingApp.Models.Dtos
{
    public class CreateRoomDto
    {
        public int HotelId { get; set; }
        public int RoomNumber { get; set; }
        public string RoomType { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
    }
}
