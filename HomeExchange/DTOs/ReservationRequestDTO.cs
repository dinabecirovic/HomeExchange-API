namespace HomeExchange.DTOs
{
    public class ReservationRequestDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int UserId { get; set; }    
        public int AdvertisementId { get; set; }
    }
}
