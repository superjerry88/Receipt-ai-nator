using System.ComponentModel.DataAnnotations;
using WebApp.DB;
using WebApp.Services;

namespace WebApp.Model;

public class UserSignUp
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email.")]
    public string Email { get; set; } = "";

    [Required]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Confirm Password is required")]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string PasswordConfirm { get; set; } = "";

    public bool MatchingPassword()
    {
        if (string.IsNullOrEmpty(Password)) return false;
        return Password == PasswordConfirm;
    }

    public async Task<bool> ValidateUsername()
    {
        if(string.IsNullOrEmpty(Username)) return false;
        var user = await RezApi.DbManager.User.GetByUsername(Username);
        return user == null;
    }

    public async Task<bool> ValidateEmail()
    {
        if(string.IsNullOrEmpty(Email)) return false;
        var user = await RezApi.DbManager.User.GetByEmail(Email);
        return user == null;
    }

    public async Task<User> Signup()
    {
        var user = new User(this);
        RezApi.Users.AllUsers.Add(user);
        await RezApi.DbManager.User.AddUser(user);
        return user;
    }
}