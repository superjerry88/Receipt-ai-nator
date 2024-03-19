using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MongoDB.Bson.Serialization.Attributes;
using System.Security.Cryptography;
using Blazorise.Extensions;
using WebApp.Services;

namespace WebApp.Model
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Salt { get; set; }
        public string PasswordHash { get; set; }
        public string ResetKey { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime RegisterDatetime { get; set; }
        public bool IsAdmin { get; set; }
        public bool HasOpenAiKey => !string.IsNullOrEmpty(EncryptedOpenAiKey);
        public string EncryptedOpenAiKey { get; set; }
        public int FreeTokenBalance { get; set; } = 50000;
        public List<ApiInfo> ApiKeys { get; set; } = new List<ApiInfo>();

        public User(UserSignUp signUp)
        {
            Id = Guid.NewGuid().ToString("N");
            ResetKey = Guid.NewGuid().ToString("N");
            Email = signUp.Email.ToLower();
            Username = signUp.Username.ToLower();
            Salt = GenerateSalt();
            PasswordHash = GetHash(signUp.Password, Salt);
            IsAdmin = Services.RezApi.Settings.AdminSignup.Contains(signUp.Username);
            RegisterDatetime = DateTime.Now;
        }

        private static string GenerateSalt()
        {
            var salt = new byte[128 / 8]; 
            RandomNumberGenerator.Create().GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        private static string GetHash(string password, string salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }

        public bool CheckPassword(string password)
        {
            return GetHash(password, Salt) == PasswordHash;
        }

        public string GetApiKey()
        {
            return EncryptedOpenAiKey.IsNullOrEmpty() ? "" : Services.RezApi.Aes.DecryptString(EncryptedOpenAiKey);
        }

        public async Task<ApiInfo> CreateNewRezApi(string name)
        { 
            var api = new ApiInfo(this)
            {
                Name = name
            };
            ApiKeys.Add(api);
            await RezApi.DbManager.User.AddOrUpdateUser(this);
            return api;
        }

        public async Task RevokeKey(ApiInfo key)
        {
            key.IsActive = false;
            await RezApi.DbManager.User.AddOrUpdateUser(this);
        }
    }

    public class ApiInfo{
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string ApiKey { get; set; } = "";
        public bool IsActive { get; set; }
        public string UserId { get; set; } = "";

        public ApiInfo()
        {
            
        }

        public ApiInfo(User user)
        {
            ApiKey = $"DEMOKEY_{DateTime.Now.Ticks}_{Guid.NewGuid():N}";
            CreatedAt = DateTime.Now;
            IsActive = true;
            UserId = user.Id;
        }
    }
}
