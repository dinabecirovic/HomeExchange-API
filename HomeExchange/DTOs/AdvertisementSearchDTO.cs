namespace HomeExchange.DTOs
{
    public class AdvertisementSearchDTO
    {
        public string? City { get; set; }
        public string? Country { get; set; }
        public int? MinRooms { get; set; }
        public float? MinArea { get; set; }
        public bool? Garden { get; set; }
        public bool? SwimmingPool { get; set; }
        public bool? ParkingSpace { get; set; }
    }
}
