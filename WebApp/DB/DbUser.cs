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

    public async Task AddOrUpdateUser(User user)
    {
        await Collection.ReplaceOneAsync(x => x.Id == user.Id, user, new ReplaceOptions { IsUpsert = true });
    }

    public async Task AddOrUpdateUserMany(List<User> users)
    {
        var bulk = new List<WriteModel<User>>();
        foreach (var user in users)
        {
            bulk.Add(new ReplaceOneModel<User>(Builders<User>.Filter.Eq(x => x.Id, user.Id), user) { IsUpsert = true });
        }
        await Collection.BulkWriteAsync(bulk);
    }

    public async Task<User?> Login(string username, string password)
    {
        var user = await Collection.AsQueryable().Where(c => c.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();
        if (user == null) return null;
        if (!user.CheckPassword(password)) return null;

        user.LastLogin = DateTime.Now;
        await AddOrUpdateUser(user);

        return user;
    }

    public async Task<List<User>> GetAllUser()
    {
        return await Collection.AsQueryable().ToListAsync();
    }

    public async Task<User> GetUser(string userId)
    {
        return await Collection.AsQueryable().FirstOrDefaultAsync(x => x.Id == userId);
    }

    public async Task<User?> GetByUsername(string username)
    {
        return await Collection.AsQueryable().FirstOrDefaultAsync(x => x.Username == username.ToLower());
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await Collection.AsQueryable().FirstOrDefaultAsync(x => x.Email == email.ToLower());
    }
}