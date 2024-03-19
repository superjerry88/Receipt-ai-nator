using WebApp.DB.Core;

namespace WebApp.Services
{
    public static class RezApi
    {
        public static SiteSetting Settings { get; set; } = new SiteSetting();
        public static FileService Files { get; set; } = new FileService();
        public static DbManager DbManager { get; set; } = new DbManager();
        public static JobService Jobs { get; set; } = new JobService();
        public static JwtHelper JwtHelper { get; set; } = new JwtHelper();

        internal static void Setup()
        {
             DbManager.InitializeDb();
             Files.Initialize();

            //probably not the best way to do this, but time is essence
            //todo improve runtime caching
            Jobs.Initialize().GetAwaiter().GetResult();
        }
    }
}
