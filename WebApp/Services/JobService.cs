using WebApp.Engine;
using WebApp.Model;
using WebApp.Model.Receipts;

namespace WebApp.Services;

public class JobService
{
    public List<ScanTask> OcrJobs { get; set; } = new List<ScanTask>();

    private async Task<ScanTask> AddJob(IJob clientType)
    {
        var job = new ScanTask()
        {
            Job = clientType
        };
        await RezApi.DbManager.ScanTask.AddTask(job);
        OcrJobs.Add(job);
        return job;
    }

    public async Task Initialize()
    {
        OcrJobs = await RezApi.DbManager.ScanTask.GetAllTask();

        //Something is wrong with these jobs, let's terminate them
        var incompleteJobs = OcrJobs.Where(x => !x.IsCompleted).ToList();
        if (incompleteJobs.Count > 0)
        {
            foreach (var job in incompleteJobs)
            {
                job.IsTerminated = true;
            }
            await RezApi.DbManager.ScanTask.AddOrUpdateManyTask(incompleteJobs);
        }


    }

    public ScanTask? GetJob(string taskId)
    {
        return OcrJobs.FirstOrDefault(x => x.Id == taskId);
    }

    public List<ScanTask> GetJobsByUser(User user)
    {
        return OcrJobs.Where(x => x.Image.UserId == user.Id).ToList();
    }

    public List<Receipt> GetReceiptByUser(User user)
    {
        var jobs = GetJobsByUser(user);
        return jobs.SelectMany(c => c.GetReceiptsWithReference()).ToList();
    }

    public async Task<ScanTask> GetGptClient()
    {
        return await AddJob(new GptReceiptClient(RezApi.Settings.OpenAiApiKey));
    }

    public async Task<ScanTask> GetTestClient()
    {
        return await AddJob(new TestClient());
    }

    public async Task<ScanTask> GetGptClient(User user)
    {
        return await AddJob(new GptReceiptClient(user.GetApiKey()));
    }
}