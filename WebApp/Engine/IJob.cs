using WebApp.Model.Receipts;

namespace WebApp.Engine;

public interface IJob
{
    public Task<ScanResult> ExtractImage(string imagePath);
}