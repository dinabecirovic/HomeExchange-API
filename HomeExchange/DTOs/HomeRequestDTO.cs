namespace HomeExchange.DTOs
{
    public class HomeRequestDTO
    {
        public List<string> UrlPhotos { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int NumberOfRooms { get; set; }
        public float HomeArea { get; set; }
        public bool Garden { get; set; }
        public bool ParkingSpace { get; set; }
        public bool SwimmingPool { get; set; }
    }
}
