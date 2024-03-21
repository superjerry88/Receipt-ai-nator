using System.Diagnostics;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
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
        await RezApi.DbManager.ScanTask.AddOrUpdateTask(this);
    }
}