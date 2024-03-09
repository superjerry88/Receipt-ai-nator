using MongoDB.Driver;
using WebApp.DB.Core;
using WebApp.Model;
using MongoDB.Driver.Linq;

namespace WebApp.DB;

public class DbUser : DbBase
{
    public DbUser(MongoConnector connector) : base(connector)
    {

    }

    private IMongoCollection<User> Collection => GetCollection<User>();

    public async Task AddUser(User user)
    {
        await Collection.InsertOneAsync(user);
    }

    public async Task<List<User>> GetAllUser()
    {
        return await Collection.AsQueryable().ToListAsync();
    }

    public async Task<User> GetUser(string userId)
    {
        return await Collection.AsQueryable().FirstOrDefaultAsync(x => x.Id == userId);
    }

    public async Task<User> GetByUsername(string username)
    {
        return await Collection.AsQueryable().FirstOrDefaultAsync(x => x.Username == username);
    }
}