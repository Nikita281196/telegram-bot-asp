namespace TelegramBot.Models
{
    public class User: Person
    {
        public long Id { get; set; }
        public string? Email { get; set; }
        public bool IsPremiumActive { get; set; }
        public List<int> TraiderIds { get; set; }
        public bool IsAdmin { get; set; }
        public User() 
        { 
            TraiderIds = new List<int>();
        }
    }
}
