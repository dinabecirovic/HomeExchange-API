namespace HomeExchange.Data.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string Roles { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsApproved { get; set; } = false;
    }
}
