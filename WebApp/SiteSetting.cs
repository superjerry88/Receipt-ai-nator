using System.Security.Principal;

namespace WebApp
{
    public class SiteSetting
    {
        public string DbConnectionString { get; set; } = "";

        public string DbHost { get; set; } = "";
        public int DbPort { get; set; }
        public string DbAuth { get; set; } = "";
        public string DbUsername { get; set; } = "";
        public string DbPassword { get; set; } = "";

        public string DbName { get; set; } = "";

        public string StoragePath { get; set; } = "";


        public string OpenAiApiKey { get; set; } = "";

        /// <summary>
        /// Resetting the JWT Seed will invalidate all the existing tokens
        /// </summary>
        public string JwtSeed { get; set; } = "";
        public string Secret { get; set; } = "";

        public List<string> AdminSignup { get; set; } = new List<string>();

        public bool UseMock { get; set; } = false;

        //Temporary

        /// <summary>
        /// For Python Image Processing Endpoint
        /// </summary>
        public string ImageProcessingEndpoint { get; set; } = "";

        /// <summary>
        /// For Python OCR Processing Endpoint
        /// </summary>
        public string OcrProcessingEndpoint { get; set; } = "";

        public long MaxImageSize { get; set; } = 0;

    }
}
