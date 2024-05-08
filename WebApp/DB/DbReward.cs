using MongoDB.Driver;
using WebApp.DB.Core;
using WebApp.Model;

namespace WebApp.DB
{
    public class DbReward(MongoConnector connector) : DbBase(connector)
    {
        private IMongoCollection<Reward> Collection => GetCollection<Reward>();

        public async Task AddReward(Reward reward)
        {
            await Collection.InsertOneAsync(reward);
        }

        public async Task<List<Reward>> GetRewards(string userId)
        {
            return await Collection.Find(r => r.UserId == userId).ToListAsync();
        }
    }
}
