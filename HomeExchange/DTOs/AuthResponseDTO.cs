namespace HomeExchange.DTOs
{
    public class AuthResponseDTO
    {
        public UserResponseDTO Users { get; set; }
        public string Token { get; set; }

        public string Role { get; set; }
    }
}
