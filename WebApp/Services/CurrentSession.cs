﻿using WebApp.DB;
using WebApp.Model;

namespace WebApp.Services;

public class CurrentSession(CookieService cookieService)
{
    public User? User { get; set; }

    public bool IsLogin { get; set; }

    public async Task<User?> GetUser()
    {
        if (User == null)
        {
            var cookie = await cookieService.GetSession();
            var userId = RezApi.Jwt.Validate(cookie);

            //check cookie
            if (userId != null)
            {
                User = await RezApi.DbManager.User.GetUser(userId);
                if (User != null)
                {
                    User.LastLogin = DateTime.Now;
                    await RezApi.DbManager.User.AddOrUpdateUser(User);
                    IsLogin = true;
                }

            }
        }

        if (User == null) return null;
        return await RezApi.DbManager.User.GetUser(User.Id);
    }

    public async Task<bool> IsAdmin()
    {
        var user = await GetUser();
        return user.IsAdmin;
    }

    public async Task ClearSession()
    {
        await cookieService.DeleteCookie();
        User = null;
    }
}