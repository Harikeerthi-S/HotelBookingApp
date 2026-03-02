namespace HotelBookingApp.Models.Dtos
{
    public class CreateAmenityDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
    }
}
