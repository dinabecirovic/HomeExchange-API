namespace HomeExchange.DTOs
{
    public class AdvertisementUpdateDTO
    {
        public string Description { get; set; } = string.Empty;
        public bool Garden { get; set; }
        public bool SwimmingPool { get; set; }
        public bool ParkingSpace { get; set; }
    }
}
