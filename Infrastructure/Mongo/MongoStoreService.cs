using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FahmiNotionAutomation.Infrastructure.Mongo
{

    public interface IMongoStoreService
    {
        Task Store<T>(T obj, string collectionName) where T : IMongoData;
        Task StoreMany<T>(IEnumerable<T> obj, string collectionName) where T : IMongoData;
        Task<T?> GetLatest<T>(string collectionName) where T : IMongoData;
        Task<IEnumerable<T>?> GetAll<T>(string collectionName) where T : IMongoData;
    }

    public class MongoStoreService : IMongoStoreService
    {
        private readonly Config _config;
        private readonly IMongoClient _mongo;

        public MongoStoreService(Config config, IMongoClient mongo)
        {
            _config = config;
            _mongo = mongo;
        }

        public async Task Store<T>(T obj, string collectionName) where T : IMongoData
        {
            var db = _mongo.GetDatabase(_config.MongoDatabase);
            var collection = db.GetCollection<T>(collectionName);
            await collection.InsertOneAsync(obj);
        }

        public async Task StoreMany<T>(IEnumerable<T> obj, string collectionName) where T : IMongoData
        {
            var db = _mongo.GetDatabase(_config.MongoDatabase);
            var collection = db.GetCollection<T>(collectionName);
            await collection.InsertManyAsync(obj);
        }

        public async Task<T?> GetLatest<T>(string collectionName) where T : IMongoData
        {
            var db = _mongo.GetDatabase(_config.MongoDatabase);
            var collection = db.GetCollection<T>(collectionName);
            var count = await collection.CountDocumentsAsync(Builders<T>.Filter.Empty);

            if (count > 0)
            {
                return await collection.AsQueryable().OrderByDescending(x => x.CreatedAt).FirstAsync();
            }

            return default;
        }

        public async Task<IEnumerable<T>?> GetAll<T>(string collectionName) where T : IMongoData
        {
            var db = _mongo.GetDatabase(_config.MongoDatabase);
            var collection = db.GetCollection<T>(collectionName);
            var count = await collection.CountDocumentsAsync(Builders<T>.Filter.Empty);

            if (count > 0)
            {
                return await collection.AsQueryable().OrderByDescending(x => x.CreatedAt).ToListAsync();
            }

            return default;
        }
    }
}
