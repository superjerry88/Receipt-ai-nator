namespace WebApp.Services;

public class FileService
{
    private const string UnsafeFolderName = "unsafe";
    private const string ResponseFolderName = "response";
    public string UnsafeFolderPath => Path.Combine(RezApi.Settings.StoragePath, UnsafeFolderName);
    public string ResponseFolderPath => Path.Combine(RezApi.Settings.StoragePath, ResponseFolderName);

    public string UnsafeFolderUrl(string fileName) => Path.Combine("files", UnsafeFolderName, fileName); 
    public string ResponseFolderUrl(string fileName) => Path.Combine("files", ResponseFolderName, fileName);


    public void Initialize()
    {
        if (!Directory.Exists(RezApi.Settings.StoragePath))
        {
            Directory.CreateDirectory(RezApi.Settings.StoragePath);
        }
        Directory.CreateDirectory(UnsafeFolderPath);
        Directory.CreateDirectory(ResponseFolderPath);
    }
        
}