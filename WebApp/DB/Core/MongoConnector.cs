using MongoDB.Driver;
using WebApp.Services;

namespace WebApp.DB.Core;

public class MongoConnector
{
    public bool Connected { get; }
    public MongoClient Client { get; }
    public IMongoDatabase GeneralDatabase { get; }

    public MongoConnector()
    {
        Client = new MongoClient(GetSecuritySettingForConnection());
        Connected = false;

        try
        {
            Client.ListDatabaseNames().ToList();
            GeneralDatabase = Client.GetDatabase(RezApi.Settings.DbName);
            Connected = true;
            Console.WriteLine("Connected to DB");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DB Error: {ex.Message}");
        }
    }

    private MongoClientSettings GetSecuritySettingForConnection()
    {
        var settings = new MongoClientSettings();
        var identity = new MongoInternalIdentity(RezApi.Settings.DbAuth, RezApi.Settings.DbUsername);
        var evidence = new PasswordEvidence(RezApi.Settings.DbPassword);
        settings.Server = new MongoServerAddress(RezApi.Settings.DbHost, RezApi.Settings.DbPort);
        settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);
        return settings;
    }
}