using BackendAPIMongo.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Diagnostics;

namespace BackendAPIMongo.Repository
{

    public interface IMatchedBabyNamesRepository
    {
        public Task<MatchedBabyNames> GetMatchedBabyNames(User user, User partner);
        public Task<MatchedBabyNames> CreateMatchedUsersList(User user, User partner);
        public Task<MatchedBabyNames> AddMatchedBabyNames(User user, User partner, BabyName babyName, MatchedBabyNames matchedBabyNames);
        public Task<MatchedBabyNames> RemoveMatchedBabyNames(User user, User partner, BabyName babyName, MatchedBabyNames matchedBabyNames);
    }

    public class MatchedBabyNamesRepository : IMatchedBabyNamesRepository
    {


        private readonly IMongoCollection<MatchedBabyNames> _matchedBabyNames;

        public MatchedBabyNamesRepository(IOptions<MongoDBRestSettings> mongoDBRest)
        {
            var client = new MongoClient(mongoDBRest.Value.ConnectionString);
            var database = client.GetDatabase(mongoDBRest.Value.DatabaseName);

            _matchedBabyNames = database.GetCollection<MatchedBabyNames>(mongoDBRest.Value.MatchedBabyNamesCollectionName);
        }

        /// <summary>
        /// Used to initialize a MatchedBabyNames object for two users, with an empty LikedBabyNames list. 
        /// (Is called when a user adds a user as their partner)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="partner"></param>
        /// <returns></returns>
        public async Task<MatchedBabyNames> CreateMatchedUsersList(User user, User partner)
        {
            // Check if a matchedBabyNames document already exists for the users
            var existingMatchedBabyNames = await _matchedBabyNames.Find(mbn => mbn.Users.Contains(user) && mbn.Users.Contains(partner)).FirstOrDefaultAsync();

            if (existingMatchedBabyNames == null)
            {
                // If no document exists, create a new one
                var newMatchedBabyNames = new MatchedBabyNames
                {
                    Users = new List<User> { user, partner },
                };

                // Insert the new MatchedBabyNames object into the collection
                await _matchedBabyNames.InsertOneAsync(newMatchedBabyNames);
            }

            // If a document already exists, return it
            return existingMatchedBabyNames;
        }


        /// <summary>
        /// Used to add a BabyName to the MatchedBabyNames object's LikedBabyNames list.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="partner"></param>
        /// <param name="babyName"></param>
        /// <param name="matchedBabyNames"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public async Task<MatchedBabyNames> AddMatchedBabyNames(User user, User partner, BabyName babyName, MatchedBabyNames matchedBabyNames)
        {
            try
            {
                await _matchedBabyNames.UpdateOneAsync(
                mbn => mbn.Users.Any(u => u.Id == user.Id || u.Email == user.Email)
                && mbn.Users.Any(p => p.Id == partner.Id || p.Email == partner.Email),
                Builders<MatchedBabyNames>.Update.Push(mbn => mbn.LikedBabyNames, babyName)
                );

            } catch (System.Exception e)
            {
                throw new System.Exception(e.Message);
            }

            return matchedBabyNames;
        }


        /// <summary>
        /// Gets the MatchedBabyNames object for two users. 
        /// Can then be used to add or remove BabyNames from the LikedBabyNames list.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="partner"></param>
        /// <returns></returns>
        public async Task<MatchedBabyNames> GetMatchedBabyNames(User user, User partner)
        {
            // Find the matchedBabyNames object based on user and partner Id or Email
            var matchedBabyNamesCursor = await _matchedBabyNames.FindAsync(
                mbn => mbn.Users.Any(u => u.Id == user.Id || u.Email == user.Email)
                    && mbn.Users.Any(p => p.Id == partner.Id || p.Email == partner.Email)
            );

            // Get the matchedBabyNames object
            var matchedBabyNames = await matchedBabyNamesCursor.FirstOrDefaultAsync();

            return matchedBabyNames;
            
        }


        /// <summary>
        /// Used to remove a BabyName from the MatchedBabyNames object's LikedBabyNames list.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="partner"></param>
        /// <param name="babyName"></param>
        /// <param name="matchedBabyNames"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public async Task<MatchedBabyNames> RemoveMatchedBabyNames(User user, User partner, BabyName babyName, MatchedBabyNames matchedBabyNames)
        {
            // Remove the babyName from the matchedBabyNames object
            try
            {
                await _matchedBabyNames.UpdateOneAsync(
                mbn => mbn.Users.Any(u => u.Id == user.Id || u.Email == user.Email)
                && mbn.Users.Any(p => p.Id == partner.Id || p.Email == partner.Email),
                Builders<MatchedBabyNames>.Update.PullFilter(mbn => mbn.LikedBabyNames, bn => bn.Id == babyName.Id)
                );
            } catch
            {
                throw new System.Exception("Could not remove babyName from matchedBabyNames");
            }

            return matchedBabyNames;


        }

    }
}
