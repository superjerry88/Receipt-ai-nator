using WebApp.Model;

namespace WebApp.Services;

public class UserService
{
    public List<User> AllUsers { get; set; } = new List<User>();

    public async Task Initialize()
    {
        AllUsers = await RezApi.DbManager.User.GetAllUser();
    }

    public User? GetUser(string id)
    {
        return AllUsers.FirstOrDefault(x => x.Id == id);
    }

    public async Task ConsumeToken(string userid, int tokens)
    {
        var user = GetUser(userid);
        if (user != null)
        {
            user.FreeTokenBalance -= tokens;
            await RezApi.DbManager.User.AddOrUpdateUser(user);
        }
        else
        {
            Console.WriteLine($"Unable to find user: {userid}");
        }
    }
}