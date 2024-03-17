using MongoDB.Driver;
using OpenAI.ObjectModels.ResponseModels;
using WebApp.DB.Core;

namespace WebApp.DB;

public class DbGptUsage(MongoConnector connector) : DbBase(connector)
{
    private IMongoCollection<ChatCompletionCreateResponse> Collection => GetCollection<ChatCompletionCreateResponse>();

    public async Task AddUsage(ChatCompletionCreateResponse usage)
    {
        await Collection.InsertOneAsync(usage);
    }

    public async Task<List<ChatCompletionCreateResponse>> GetAllUsage()
    {
        return await Collection.AsQueryable().ToListAsync();
    }
}