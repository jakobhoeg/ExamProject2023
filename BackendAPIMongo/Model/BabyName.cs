using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackendAPIMongo.Model
{
    public class BabyName
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsMale { get; set; }
        public bool IsFemale { get; set; }
        public bool IsInternational { get; set; }
        public int AmountOfLikes { get; set; }

    }
}
