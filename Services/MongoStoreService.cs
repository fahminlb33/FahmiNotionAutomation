using FahmiNotionAutomation.Infrastructure;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Threading.Tasks;

namespace FahmiNotionAutomation.Services
{
    public interface IMongoStoreService
    {
        Task Store(KanbanPerformance performance);
        Task Store(KanbanStatistics statistics);
        Task<KanbanStatistics?> GetLatestStatistics();
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

        public async Task Store(KanbanStatistics statistics)
        {
            var db = _mongo.GetDatabase(_config.MongoDatabase);
            var collection = db.GetCollection<KanbanStatistics>("statistics");
            await collection.InsertOneAsync(statistics);
        }

        public async Task<KanbanStatistics?> GetLatestStatistics()
        {
            var db = _mongo.GetDatabase(_config.MongoDatabase);
            var collection = db.GetCollection<KanbanStatistics>("statistics");
            var count = await collection.CountDocumentsAsync(Builders<KanbanStatistics>.Filter.Empty);

            if (count > 0)
            {
                return await collection.AsQueryable().OrderByDescending(x => x.Period).FirstAsync();
            }

            return null;
        }

        public async Task Store(KanbanPerformance performance)
        {
            var db = _mongo.GetDatabase(_config.MongoDatabase);
            var collection = db.GetCollection<KanbanPerformance>("performance");
            await collection.InsertOneAsync(performance);
        }
    }
}
