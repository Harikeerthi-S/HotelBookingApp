namespace HotelBookingApp.Models.Dtos
{
    public class CreateCancellationDto
    {
        public int BookingId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}