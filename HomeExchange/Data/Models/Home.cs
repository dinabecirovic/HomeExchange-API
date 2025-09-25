using System.ComponentModel.DataAnnotations.Schema;

namespace HomeExchange.Data.Models
{
    [NotMapped]
    public class Home
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
        public string Availability { get; set; } 
    }
}
