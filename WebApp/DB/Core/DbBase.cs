using MongoDB.Driver;

namespace WebApp.DB.Core;

public abstract class DbBase(MongoConnector connector)
{
    private string? _collectionName;

    protected IMongoCollection<T> GetCollection<T>()
    {
        var collectionName = $"{GetCollectionBaseName()}.{typeof(T).Name}";
        return connector.GeneralDatabase.GetCollection<T>(collectionName);
    }

    protected IMongoCollection<T> GetCollection<T>(string subName)
    {
        return connector.GeneralDatabase.GetCollection<T>($"{GetCollectionBaseName()}.{subName}");
    }

    private string? GetCollectionBaseName()
    {
        if (_collectionName != null) return _collectionName;

        var attribute = (CollectionName)Attribute.GetCustomAttribute(GetType(), typeof(CollectionName));
        _collectionName = attribute != null ? attribute.Value : GetType().FullName;
        return _collectionName;
    }
}