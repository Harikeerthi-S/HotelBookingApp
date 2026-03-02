namespace HotelBookingApp.Models.Dtos
{
    public class CreateHotelDto
    {
        public string HotelName { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double StarRating { get; set; }
        public string ContactNumber { get; set; } = string.Empty;
    }

   
}
