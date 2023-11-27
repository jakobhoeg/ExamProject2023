using MongoDB.Bson.Serialization.Attributes;

namespace BackendAPIMongo.Model
{
    public class BabyName
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsMale { get; set; }
        public bool IsFemale { get; set; }
        public bool IsInternational { get; set; }
        public int AmountOfLikes { get; set; }

    }
}
