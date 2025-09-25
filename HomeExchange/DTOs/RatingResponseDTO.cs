namespace HomeExchange.DTOs
{
    public class RatingResponseDTO
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; }
        public int UserId { get; set; }
        public int AdvertisementId { get; set; }

        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
    }
}
