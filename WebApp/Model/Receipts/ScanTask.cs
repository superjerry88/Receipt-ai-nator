using System.Diagnostics;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using WebApp.DB.Core;
using WebApp.Engine;
using WebApp.Model.Images;
using WebApp.Services;

namespace WebApp.Model.Receipts;

public class ScanTask
{
    [BsonId]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsCompleted { get; set; } = false;
    public bool IsError { get; set; } = false;
    public bool IsTerminated { get; set; } = false;
    public string ErrorMessage { get; set; } = "";
    public string ErrorStackTrace { get; set; } = "";
    public ScanResult? Result { get; set; }
    public TimeSpan TimeTaken { get; set; }
    public ImageInfo Image { get; set; }

    private const int RewardPerReceipt = 5;

    //welcome to code horror... Bson for mongodb, Json for newtonsoft, System.Text.Json for swagger...
    [BsonIgnore]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public IJob Job { get; set; } = new TestClient();

    [BsonIgnore]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public EventHandler<ScanTask> OnComplete { get; set; }

    [BsonIgnore]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public EventHandler<Exception> OnError { get; set; }

    public async Task DoJob(ImageInfo image)
    {
        var sw = Stopwatch.StartNew();
        Image = image;
        await RezApi.DbManager.ScanTask.AddOrUpdateTask(this);
        IsCompleted = false;
        IsError = false;
        try
        {
            Result = await Job.ExtractImage(image.LocalFilePath);
            IsCompleted = true;
            TimeTaken = sw.Elapsed;
            OnComplete?.Invoke(this, this);
        }
        catch (Exception e)
        {
            IsError = true;
            IsCompleted = true;
            TimeTaken = sw.Elapsed;
            ErrorMessage = e.Message;
            ErrorStackTrace = e.StackTrace!;
            OnError?.Invoke(this, e);
        }
        if (Result is { TokenConsumed: > 0, UsingSiteToken: true })
        {
            await RezApi.Users.ConsumeToken(Image.UserId, Result.TokenConsumed);
        }

        await GetReward();
        await RezApi.DbManager.ScanTask.AddOrUpdateTask(this);
    }

    public List<Receipt> GetReceiptsWithReference()
    {
        if (Result == null)
        {
            return [];
        }
        var receipts = Result.Receipts;
        for (var i = 0; i < receipts.Count; i++)
        {
            var receipt = receipts[i];
            receipt.TaskId = Id;
            receipt.ReceiptIndex = i;
        }
        return receipts;
    }

    public async Task GetReward()
    {
        if (Result == null) return;
        foreach (var receipt in Result.Receipts)
        {
            var reward = RewardFactory.CreateReward(Image.UserId,receipt.ReceiptId, $"Scan Reward - {receipt.ShopName}", RewardType.ScanReward, RewardPerReceipt);
            await RezApi.DbManager.Reward.AddReward(reward);

            var user = RezApi.Users.GetUser(Image.UserId);
            if (user != null) await user.AddReward(RewardPerReceipt);
        }
    }
}