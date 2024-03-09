using MongoDB.Bson.Serialization.Attributes;

namespace WebApp.Model
{
    /// <summary>
    /// Simple user object
    /// </summary>
    public class User
    {
        [BsonId]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Username { get; set; }
        public string HashedPassword { get; set; }
    }
}
