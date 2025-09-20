namespace HomeExchange.Data.Models
{
    public class Advertisement
    {
        public int Id { get; set; }
        public List<string> UrlPhotos { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int NumberOfRooms { get; set; }
        public float HomeArea { get; set; }
        public bool Garden { get; set; }
        public bool ParkingSpace { get; set; }
        public bool SwimmingPool { get; set; }
        public string Availability { get; set; }
        public bool IsApproved { get; set; } = false;

        public int HomeOwnerId { get; set; }
        public Users HomeOwner { get; set; } = null!;

    }
}
