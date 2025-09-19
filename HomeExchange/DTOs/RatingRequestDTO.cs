using HomeExchange.Data.Models;

namespace HomeExchange.DTOs
{
    public class RatingRequestDTO
    {
        public int Score { get; set; }
        public string Comment { get; set; }
        public int HomeOwnerId { get; set; }
        public int AdvertisementId { get; set; }
    }
}
