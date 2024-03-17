using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MongoDB.Bson.Serialization.Attributes;
using System.Security.Cryptography;
using WebApp.Services;

namespace WebApp.Model
{
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

        public User(UserSignUp signUp)
        {
            Id = Guid.NewGuid().ToString("N");
            ResetKey = Guid.NewGuid().ToString("N");
            Email = signUp.Email.ToLower();
            Username = signUp.Username.ToLower();
            Salt = GenerateSalt();
            PasswordHash = GetHash(signUp.Password, Salt);
            IsAdmin = RezApi.Settings.AdminSignup.Contains(signUp.Username);
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
    }
}
