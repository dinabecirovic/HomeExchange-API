namespace HomeExchange.DTOs
{
    public class ReservationResponseDTO
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int AdvertisementId { get; set; }

        public int UserId { get; set; }
        public string FirstName { get; set; }  
        public string LastName { get; set; }    
        public string Email { get; set; }       
    }
}
