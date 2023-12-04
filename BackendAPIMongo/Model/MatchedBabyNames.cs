using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackendAPIMongo.Model
{
    public class MatchedBabyNames
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public List<User> Users { get; set; }
        public List<BabyName> LikedBabyNames { get; set; }
        public MatchedBabyNames()
        {
            Users = new List<User>();
            LikedBabyNames = new List<BabyName>();
        }
    }
}
