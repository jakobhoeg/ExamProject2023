﻿using BackendAPIMongo.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Diagnostics;

namespace BackendAPIMongo.Repository
{
    public interface IBabyNameRepository
    {
        public Task<List<BabyName>> GetBabyNames(int pageIndex);
    }

    public class BabyNameRepository : IBabyNameRepository
    {
        private readonly IMongoCollection<BabyName> _babyNames;
        private const int pageSize = 30;

        public BabyNameRepository(IOptions<MongoDBRestSettings> mongoDBRest)
        {
            var client = new MongoClient(mongoDBRest.Value.ConnectionString);
            var database = client.GetDatabase(mongoDBRest.Value.DatabaseName);

            _babyNames = database.GetCollection<BabyName>(mongoDBRest.Value.BabyNameCollectionName);
            var keys = Builders<BabyName>.IndexKeys.Ascending(bn => bn.Name);
            _babyNames.Indexes.CreateOne(new CreateIndexModel<BabyName>(keys));
        }

        /// <summary>
        /// Keeps track of which babyname page the user is on and skips a 
        /// fixed number of names to return the right names.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public Task<List<BabyName>> GetBabyNames(int pageIndex)
        {
            var skipCount = (pageIndex - 1) * pageSize;

            var babyNamesList = _babyNames.Find(b => true)
                                           .Skip(skipCount)
                                           .Limit(pageSize)
                                           .ToList();

            return Task.FromResult(babyNamesList);
        }

    }
}