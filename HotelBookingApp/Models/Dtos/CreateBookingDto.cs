namespace HotelBookingApp.Models.Dtos
{
    public class CreateBookingDto
    {
        public int UserId { get; set; }
        public int HotelId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
