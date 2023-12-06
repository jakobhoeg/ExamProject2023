﻿using BackendAPIMongo.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Diagnostics;

namespace BackendAPIMongo.Repository
{
    public interface IBabyNameRepository
    {
        public Task<List<BabyName>> GetBabyNames(int pageIndex);
        public Task<List<BabyName>> GetBabyNames(int pageIndex, bool isMale, bool isFemale, bool isInternational);
        public Task<List<BabyName>> GetInternationalBabyNames(int pageIndex, bool isInternational);
        public Task<List<BabyName>> GetBabyNamesSortedByLikesAsc(int pageIndex, bool isMale, bool isFemale, bool isInternational);
        public Task<List<BabyName>> GetBabyNamesSortedByLikesDesc(int pageIndex, bool isMale, bool isFemale, bool isInternational);
        public Task<List<BabyName>> GetBabyNamesSortedByNameAsc(int pageIndex, bool isMale, bool isFemale, bool isInternational);
        public Task<List<BabyName>> GetBabyNamesSortedByNameDesc(int pageIndex, bool isMale, bool isFemale, bool isInternational);

        public Task<long> AddLike(BabyName babyName);
        public Task<long> RemoveLike(BabyName babyName);
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

        #region Filters and Sorting methods

        /// <summary>
        /// Returns the right names based on gender and internationality and uses pagination.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="isMale"></param>
        /// <param name="isFemale"></param>
        /// <param name="isInternational"></param>
        /// <returns></returns>
        public Task<List<BabyName>> GetBabyNames(int pageIndex, bool isMale, bool isFemale, bool isInternational)
        {
            var skipCount = (pageIndex - 1) * pageSize;

            var filterBuilder = Builders<BabyName>.Filter;
            var filter = filterBuilder.Eq(b => b.IsMale, isMale) & 
                 filterBuilder.Eq(b => b.IsFemale, isFemale) 
                & filterBuilder.Eq(b => b.IsInternational, isInternational);

            var babyNamesList = _babyNames.Find(filter)
                                           .SortBy(b => b.Name)
                                           .Skip(skipCount)
                                           .Limit(pageSize)
                                           .ToList();

            return Task.FromResult(babyNamesList);
        }

        public Task<List<BabyName>> GetInternationalBabyNames(int pageIndex, bool isInternational)
        {
            var skipCount = (pageIndex - 1) * pageSize;

            var filterBuilder = Builders<BabyName>.Filter;
            var filter = filterBuilder.Eq(b => b.IsInternational, isInternational);

            var babyNamesList = _babyNames.Find(filter)
                                           .SortBy(b => b.Name)
                                           .Skip(skipCount)
                                           .Limit(pageSize)
                                           .ToList();

            return Task.FromResult(babyNamesList);
        }

        public Task<List<BabyName>> GetBabyNamesSortedByLikesAsc(int pageIndex, bool isMale, bool isFemale, bool isInternational)
        {
            var skipCount = (pageIndex - 1) * pageSize;

            var filterBuilder = Builders<BabyName>.Filter;
            var filter = filterBuilder.Eq(b => b.IsMale, isMale)
                & filterBuilder.Eq(b => b.IsFemale, isFemale)
                & filterBuilder.Eq(b => b.IsInternational, isInternational);

            var babyNamesList = _babyNames.Find(filter)
                                           .SortBy(b => b.AmountOfLikes)
                                           .Skip(skipCount)
                                           .Limit(pageSize)
                                           .ToList();

            return Task.FromResult(babyNamesList);
        }

        public Task<List<BabyName>> GetBabyNamesSortedByLikesDesc(int pageIndex, bool isMale, bool isFemale, bool isInternational)
        {
            var skipCount = (pageIndex - 1) * pageSize;

            var filterBuilder = Builders<BabyName>.Filter;
            var filter = filterBuilder.Eq(b => b.IsMale, isMale)
                & filterBuilder.Eq(b => b.IsFemale, isFemale)
                & filterBuilder.Eq(b => b.IsInternational, isInternational);

            var babyNamesList = _babyNames.Find(filter)
                                           .SortByDescending(b => b.AmountOfLikes)
                                           .Skip(skipCount)
                                           .Limit(pageSize)
                                           .ToList();

            return Task.FromResult(babyNamesList);
        }

        public Task<List<BabyName>> GetBabyNamesSortedByNameAsc(int pageIndex, bool isMale, bool isFemale, bool isInternational)
        {
            var skipCount = (pageIndex - 1) * pageSize;

            var filterBuilder = Builders<BabyName>.Filter;
            var filter = filterBuilder.Eq(b => b.IsMale, isMale)
                & filterBuilder.Eq(b => b.IsFemale, isFemale)
                & filterBuilder.Eq(b => b.IsInternational, isInternational);

            var babyNamesList = _babyNames.Find(filter)
                                           .SortBy(b => b.Name)
                                           .Skip(skipCount)
                                           .Limit(pageSize)
                                           .ToList();

            return Task.FromResult(babyNamesList);
        }

        public Task<List<BabyName>> GetBabyNamesSortedByNameDesc(int pageIndex, bool isMale, bool isFemale, bool isInternational)
        {
            var skipCount = (pageIndex - 1) * pageSize;

            var filterBuilder = Builders<BabyName>.Filter;
            var filter = filterBuilder.Eq(b => b.IsMale, isMale)
                & filterBuilder.Eq(b => b.IsFemale, isFemale)
                & filterBuilder.Eq(b => b.IsInternational, isInternational);

            var babyNamesList = _babyNames.Find(filter)
                                           .SortByDescending(b => b.Name)
                                           .Skip(skipCount)
                                           .Limit(pageSize)
                                           .ToList();

            return Task.FromResult(babyNamesList);
        }

        #endregion

        /// <summary>
        /// Increments the amount of likes of a babyname by one.
        /// </summary>
        /// <param name="babyName"></param>
        /// <returns></returns>
        public Task<long> AddLike(BabyName babyName)
        {
            var update = Builders<BabyName>.Update.Inc(bn => bn.AmountOfLikes, 1);
            var result = _babyNames.UpdateOne(bn => bn.Name == babyName.Name, update);

            return Task.FromResult(result.ModifiedCount);
        }

        public Task<long> RemoveLike(BabyName babyName)
        {
            var update = Builders<BabyName>.Update.Inc(bn => bn.AmountOfLikes, -1);
            var result = _babyNames.UpdateOne(bn => bn.Name == babyName.Name, update);

            return Task.FromResult(result.ModifiedCount);
        }


    }
}
