using System.ComponentModel.DataAnnotations;

namespace HomeExchange.Data.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int Score { get; set; }

        public string Comment { get; set; }

        public int UserId { get; set; }
        public Users User { get; set; }

        public int AdvertisementId { get; set; }
        public Advertisement Advertisement { get; set; }
    }
}
