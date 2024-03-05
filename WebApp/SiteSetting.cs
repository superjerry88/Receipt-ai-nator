namespace WebApp
{
    public class SiteSetting
    {
        public string DbConnectionString { get; set; } = "";

        public string DbName { get; set; } = "";

        public string StoragePath { get; set; } = "";


        public string OpenAiApiKey { get; set; } = "";

        /// <summary>
        /// Resetting the JWT Seed will invalidate all the existing tokens
        /// </summary>
        public string JwtSeed { get; set; } = "";

        //Temporary

        /// <summary>
        /// For Python Image Processing Endpoint
        /// </summary>
        public string ImageProcessingEndpoint { get; set; } = "";

        /// <summary>
        /// For Python OCR Processing Endpoint
        /// </summary>
        public string OcrProcessingEndpoint { get; set; } = "";

    }
}
