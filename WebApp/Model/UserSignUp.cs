using WebApp.DB;
using WebApp.Services;

namespace WebApp.Model;

public class UserSignUp
{
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string PasswordConfirm { get; set; } = "";

    public bool MatchingPassword()
    {
        if (string.IsNullOrEmpty(Password)) return false;
        return Password == PasswordConfirm;
    }

    public async Task<bool> ValidateUsername()
    {
        if(string.IsNullOrEmpty(Username)) return false;
        var user = await RezApi.DbManager.Get<DbUser>().GetByUsername(Username);
        return user == null;
    }

    public async Task<bool> ValidateEmail()
    {
        if(string.IsNullOrEmpty(Email)) return false;
        var user = await RezApi.DbManager.Get<DbUser>().GetByEmail(Email);
        return user == null;
    }

    public async Task<User> Signup()
    {
        var user = new User(this);
        await RezApi.DbManager.Get<DbUser>().AddUser(user);
        return user;
    }
}