namespace AdminClient.Models
{
    public class BabyName
    {
        public string Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsMale { get; set; }
        public bool IsFemale { get; set; }
        public bool IsInternational { get; set; }
        public int AmountOfLikes { get; set; }

    }
}
