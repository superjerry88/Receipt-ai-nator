using Microsoft.JSInterop;
using WebApp.DB.Core;

namespace WebApp.Services;

public class CookieService(IJSRuntime jsRuntime)
{
    private const string CookieName = "Session";

    public async Task<string> GetSession()
    {
        return await jsRuntime.InvokeAsync<string>("localStorage.getItem", CookieName);
    }

    public async Task SetCookie(string currency)
    {
        await jsRuntime.InvokeAsync<object>("localStorage.setItem", CookieName, currency);
    }

    public async Task DeleteCookie()
    {
        await jsRuntime.InvokeAsync<object>("localStorage.removeItem", CookieName);
    }
}