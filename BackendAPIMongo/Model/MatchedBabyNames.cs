using MongoDB.Bson.Serialization.Attributes;

namespace BackendAPIMongo.Model
{
    public class MatchedNames
    {
        [BsonId]
        public Guid Id { get; set; }
        public List<User> Users { get; set; }
        public List<BabyName> LikedBabyNames { get; set; }
        public MatchedNames()
        {
            Users = new List<User>();
            LikedBabyNames = new List<BabyName>();
        }
    }
}
