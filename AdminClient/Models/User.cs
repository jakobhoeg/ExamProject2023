namespace AdminClient.Models
{
    public class User
    {
        
        public string Id { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public bool IsAdmin { get; set; }
        public DateTime CreatedDate { get; set; }

        public User? Partner { get; set; } = null!;

        public List<BabyName> LikedBabyNames { get; set; }

        public User()
        {
            LikedBabyNames = new List<BabyName>();
        }
    }
}
