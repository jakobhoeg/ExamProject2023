using BackendAPIMongo.Model;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;

namespace BackendAPIMongo.Repository
{

    public interface IUserRepository
    {
        // Methods mostly intended for users
        public Task<bool> Authenticate(User user);
        public Task<bool> AuthenticateAdmin(User user);
        public Task Register(User user);
        public Task<User> GetUser(User user);
        public Task AddPartner(User user, string partnerEmail);
        public Task RemovePartner(User user, string partnerEmail);
        public Task LikeBabyname(User user, BabyName babyName);
        public Task UnlikeBabyname(User user, BabyName babyName);
        public Task ChangeEmailAddressAsync(User user, string newEmail);

        // Admin methods
        public Task<long> GetUserCountAsync();
    }

    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IOptions<MongoDBRestSettings> mongoDBRest)
        {
            var client = new MongoClient(mongoDBRest.Value.ConnectionString);
            var database = client.GetDatabase(mongoDBRest.Value.DatabaseName);

            _users = database.GetCollection<User>(mongoDBRest.Value.UserCollectionName);
        }

        #region User handling
        /// <summary>
        /// Authenticates user by checking if user exists and if password is correct.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task<bool> Authenticate(User user)
        {
            // Check if user exists
            var userExists = _users.Find(u => u.Email == user.Email).FirstOrDefault();

            if (userExists == null)
            {
                throw new Exception("User does not exist");
            }

            // Check if password is correct (BCrypt)
            if (!BCrypt.Net.BCrypt.Verify(user.Password, userExists.Password))
            {
                throw new Exception("Password is incorrect");
            }

            return Task.FromResult(true);
        }

        public Task<bool> AuthenticateAdmin(User user)
        {
            // Check if user exists
            var userExists = _users.Find(u => u.Email == user.Email).FirstOrDefault();

            // Check if user is admin
            if (!userExists.IsAdmin)
            {
                throw new Exception("User is not admin");
            }

            if (userExists == null)
            {
                throw new Exception("User does not exist");
            }

            // Check if password is correct (BCrypt)
            if (!BCrypt.Net.BCrypt.Verify(user.Password, userExists.Password))
            {
                throw new Exception("Password is incorrect");
            }


            return Task.FromResult(true);
        }

        /// <summary>
        /// Registers non admin user by checking if user already exists and 
        /// then adds user to database and encrypts their password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task Register(User user)
        {
            if (_users.Find(u => u.Email == user.Email).FirstOrDefault() != null)
            {
                throw new Exception("User already exists");
            }

            // Encrypt password using BCrypt
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Add user to database
            _users.InsertOne(user);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Returns user object if user exists. 
        /// Used to retrieve user object for frontend.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task<User> GetUser(User user)
        {
            var userExists = _users.Find(u => u.Email == user.Email).FirstOrDefault();

            if (userExists == null)
            {
                throw new Exception(user.Email + " does not exist");
            }

            return Task.FromResult(userExists);
        }

        /// <summary>
        /// Adds partner to user and the other way around.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="partnerEmail"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task AddPartner(User user, string partnerEmail)
        {
            var partner = _users.Find(u => u.Email == partnerEmail).FirstOrDefault();

            if (partner == null)
            {
                throw new Exception(partnerEmail + " does not exist");
            }

            if (user.Partner != null || partner.Partner != null)
            {
                throw new Exception("User already has a partner");
            }

            try
            {
                _users.UpdateOne(u => u.Email == user.Email, Builders<User>.Update.Set(u => u.Partner, partner));
                _users.UpdateOne(u => u.Email == partnerEmail, Builders<User>.Update.Set(u => u.Partner, user));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Removes partner from user and the other way around.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="partnerEmail"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task RemovePartner(User user, string partnerEmail)
        {
            var partner = _users.Find(u => u.Email == partnerEmail).FirstOrDefault();

            if (partner == null)
            {
                throw new Exception(partnerEmail + " does not exist");
            }

            if (user.Partner == null || partner.Partner == null)
            {
                throw new Exception("User does not have a partner");
            }

            try
            {
                _users.UpdateOne(u => u.Email == user.Email, Builders<User>.Update.Set(u => u.Partner, null));
                _users.UpdateOne(u => u.Email == partnerEmail, Builders<User>.Update.Set(u => u.Partner, null));
                
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return Task.FromResult(true);
        }
        #endregion

        #region BabyName Like endpoints
        /// <summary>
        /// Add babyname to user's liked babyname list.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="babyNames"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task LikeBabyname(User user, BabyName babyName)
        {

            try
            {
                _users.UpdateOne(u => u.Email == user.Email, Builders<User>.Update.Push(u => u.LikedBabyNames, babyName));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return Task.FromResult(true);
        }

        public Task UnlikeBabyname(User user, BabyName babyName)
        {
            try
            {
                _users.UpdateOne(u => u.Email == user.Email, Builders<User>.Update.PullFilter(u => u.LikedBabyNames, bn => bn.Id == babyName.Id));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return Task.FromResult(true);
        }
        #endregion

        /// <summary>
        /// Changes a user's email address.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newEmail"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task ChangeEmailAddressAsync(User user, string newEmail)
        {
            if (_users.Find(u => u.Email == user.Email).FirstOrDefault() == null)
            {
                throw new Exception("User does not exist");
            }
            else if (_users.Find(u => u.Email == newEmail).FirstOrDefault() != null)
            {
                throw new Exception("Email already exists");
            }
            else
            {
                try
                {
                    _users.UpdateOne(u => u.Email == user.Email, Builders<User>.Update.Set(u => u.Email, newEmail));
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

            return Task.FromResult(true);
        }

        #region Admin methods

        /// <summary>
        /// Counts the amount of users in the database.
        /// </summary>
        /// <returns></returns>
        public Task<long> GetUserCountAsync()
        {
            long userCount = 0;

            try
            {
                userCount = _users.CountDocuments(new BsonDocument());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return Task.FromResult(userCount);
        }

        #endregion
    }
}