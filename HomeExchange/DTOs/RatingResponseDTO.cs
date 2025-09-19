namespace HomeExchange.DTOs
{
    public class RatingResponseDTO
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; }
        public int HomeOwnerId { get; set; }
        public int HomeId { get; set; }
    }
}
