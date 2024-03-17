using WebApp.DB.Core;

namespace WebApp.Services
{
    public static class RezApi
    {
        public static SiteSetting Settings { get; set; } = new SiteSetting();
        public static FileService Files { get; set; } = new FileService();
        public static DbManager DbManager { get; set; } = new DbManager();
        public static JwtHelper JwtHelper { get; set; } = new JwtHelper();

        internal static void Setup()
        {
             DbManager.InitializeDb();
             Files.Initialize();
        }
    }
}
