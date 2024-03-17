using JWT.Algorithms;
using JWT.Builder;
using WebApp.Model;

namespace WebApp.Services;

public class JwtHelper
{
    private string Secret => RezApi.Settings.JwtSeed;

    public string Generate(User user, int hour = 9999)
    {
        return JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA256Algorithm()) //todo outdated but im lazy
            .WithSecret(Secret)
            .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(hour).ToUnixTimeSeconds())
            .AddClaim("User", user.Id)
            .Encode();
    }

    public string? Validate(string? token)
    {
        if (token == null) return null;
        try
        {
            var payload = JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                .WithSecret(Secret)
                .MustVerifySignature()
                .Decode<IDictionary<string, object>>(token);
            return payload["User"].ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }

    }
}