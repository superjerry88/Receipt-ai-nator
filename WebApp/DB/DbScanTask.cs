using MongoDB.Driver;
using MongoDB.Driver.Linq;
using WebApp.DB.Core;
using WebApp.Model.Receipts;

namespace WebApp.DB;

public class DbScanTask : DbBase
{
    public DbScanTask(MongoConnector connector) : base(connector)
    {
    }

    private IMongoCollection<ScanTask> Collection => GetCollection<ScanTask>();

    public async Task AddTask(ScanTask task)
    {
        await Collection.InsertOneAsync(task);
    }

    public async Task AddOrUpdateTask(ScanTask task)
    {
        await Collection.ReplaceOneAsync(x => x.Id == task.Id, task, new ReplaceOptions { IsUpsert = true });
    }

    public async Task AddOrUpdateManyTask(List<ScanTask> tasks)
    {
        var bulkOps = new List<WriteModel<ScanTask>>();

        foreach (var item in tasks)
        {
            var upsertOne = new ReplaceOneModel<ScanTask>(Builders<ScanTask>.Filter.Eq(i => i.Id, item.Id), item)
            {
                IsUpsert = true
            };

            bulkOps.Add(upsertOne);
        }

        await Collection.BulkWriteAsync(bulkOps);
    }

    public async Task<List<ScanTask>> GetAllTask()
    {
        return await Collection.AsQueryable().ToListAsync();
    }

    public async Task<ScanTask> GetTask(string taskId)
    {
        return await Collection.AsQueryable().FirstOrDefaultAsync(x => x.Id == taskId);
    }
}