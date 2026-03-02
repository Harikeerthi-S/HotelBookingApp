namespace HotelBookingApp.Models.Dtos
{
    public class CreateReviewDto
    {
        public int HotelId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
