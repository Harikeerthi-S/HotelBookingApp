namespace HotelBookingApp.Models.Dtos
{
    public class CreateNotificationDto
    {
        public int UserId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
