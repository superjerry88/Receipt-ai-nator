using System.Collections.Concurrent;

namespace WebApp.DB.Core
{
    public class DbManager
    {
        private MongoConnector? Connector { get; set; }
        private bool Initialized { get; set; }
        private ConcurrentDictionary<Type, DbBase> DbInstance { get; set; } = new();

        public DbUser User => Get<DbUser>();
        public DbGptUsage GptUsage => Get<DbGptUsage>();
        public DbScanTask ScanTask => Get<DbScanTask>();
        public DbReward Reward => Get<DbReward>();


        public void InitializeDb()
        {
            if (Initialized) return;
            Connector = new MongoConnector();
            if (Connector.Connected)
            {
                Initialized = true;
            }
        }

        private T Get<T>() where T : DbBase
        {
            if (Connector == null)
            {
                throw new Exception("DbManager module is not loaded yet.");
            }
            var type = typeof(T);

            if (!DbInstance.ContainsKey(type))
            {
                var db = (T)Activator.CreateInstance(type, Connector)!;
                DbInstance.TryAdd(type, db);
            }
            return (T)DbInstance[type];
        }

        public DbBase Get(Type type)
        {
            if (Connector == null)
            {
                throw new Exception("DbManager module is not loaded yet.");
            }

            if (!DbInstance.ContainsKey(type))
            {
                var db = (DbBase)Activator.CreateInstance(type, Connector)!;
                DbInstance.TryAdd(type, db);
            }
            return DbInstance[type];
        }

    }
}
