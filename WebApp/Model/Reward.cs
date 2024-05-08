using MongoDB.Bson.Serialization.Attributes;

namespace WebApp.Model
{
    public class RewardFactory
    {
        public static Reward CreateReward(string userId,string receiptId ,string title, RewardType rewardType, int value)
        {
            return new Reward
            {
                UserId = userId,
                Title = title,
                RewardType = rewardType,
                Value = value,
                ReceiptId = receiptId
            };
        }
    }

    public class Reward
    {
        [BsonId]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string UserId { get; set; } = "";
        public string ReceiptId { get; set; } = "";
        public string Title { get; set; } = "";
        public RewardType RewardType { get; set; }
        public int Value { get; set; } 
    }

    public enum RewardType
    {
        ScanReward,
        ChatReward,
        Redeemed,
        Unknown,
    }
}
