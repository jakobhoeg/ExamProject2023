using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace BackendAPIMongo.Model
{
    public class User
    {
        [BsonId]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; } = null!;

        public bool IsAdmin { get; set; }

        public User Partner { get; set; } = null!;

        public List<BabyName> LikedBabyNames { get; set; }

        public User()
        {
            LikedBabyNames = new List<BabyName>();
            IsAdmin = false;
        }


    }
}
