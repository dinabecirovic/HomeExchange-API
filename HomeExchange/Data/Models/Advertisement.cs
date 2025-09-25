namespace HomeExchange.Data.Models
{
    public class Advertisement : Home
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsApproved { get; set; } = false;

        public int HomeOwnerId { get; set; }
        public Users HomeOwner { get; set; } = null!;
    }
}
