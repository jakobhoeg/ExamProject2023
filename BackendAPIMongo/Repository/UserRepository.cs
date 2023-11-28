using BackendAPIMongo.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Diagnostics;

namespace BackendAPIMongo.Repository
{

    public interface IUserRepository
    {
        public Task<bool> Authenticate(User user);
        public Task Register(User user);

        public Task<User> GetUser(User user);

        public Task AddPartner(User user, string partnerEmail);

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

        // Authenticate user (login)
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

        // Register user (not admin)
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

        // Get user of logged in user
        public Task<User> GetUser(User user)
        {
            var userExists = _users.Find(u => u.Email == user.Email).FirstOrDefault();

            if (userExists == null)
            {
                throw new Exception(user.Email + " does not exist");
            }

            return Task.FromResult(userExists);
        }

        public Task AddPartner(User user, string partnerEmail)
        {
            var partner = _users.Find(u => u.Email == partnerEmail).FirstOrDefault();

            if (partner == null)
            {
                throw new Exception(partnerEmail + " does not exist");
            }

            if (user.Partner != null)
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
    }
}
