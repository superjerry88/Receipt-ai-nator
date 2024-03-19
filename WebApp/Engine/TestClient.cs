using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using WebApp.Model.Receipts;

namespace WebApp.Engine;

public class TestClient : IJob
{
    public async Task<ScanResult> ExtractImage(string imagePath)
    {
        Console.WriteLine("TestClient is running");
        await Task.Delay(8000);
        var json = GetMockResult("Result1");
        Console.WriteLine("TestClient done extracting mock");
        return JsonConvert.DeserializeObject<ScanResult>(json)!;
    }

    public string GetMockResult(string file)
    {
        var resourceName = "Data.MockResult." + file;
        var resourcePath = resourceName;
        var assembly = Assembly.GetExecutingAssembly();

        if (!resourceName.StartsWith(nameof(WebApp)))
        {
            resourcePath = assembly.GetName().Name + "." + resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");
        }

        using var stream = assembly.GetManifestResourceStream(resourcePath);
        using var reader = new StreamReader(stream ?? throw new Exception("Incorrect Mock File"), Encoding.UTF8);
        return reader.ReadToEnd();
    }
}