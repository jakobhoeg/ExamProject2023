namespace BackendAPIMongo
{
    public class MongoDBRestSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string UserCollectionName { get; set; } = null!;
        public string BabyNameCollectionName { get; set; } = null!;
        public string MatchedBabyNamesCollectionName { get; set; } = null!;
    }
}