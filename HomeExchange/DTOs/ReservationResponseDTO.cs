namespace HomeExchange.DTOs
{
    public class ReservationResponseDTO
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int UserId { get; set; }
        public int HomeId { get; set; }
    }
}
