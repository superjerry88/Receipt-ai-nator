using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using WebApp.Model;
using WebApp.Services;

namespace WebApp.Engine
{
    public class AiManager
    {
        public string GetPrompt(string file)
        {
            var resourceName = "Prompt." + file;
            var resourcePath = resourceName;
            var assembly = Assembly.GetExecutingAssembly();

            if (!resourceName.StartsWith(nameof(WebApp)))
            {
                resourcePath = assembly.GetName().Name + "." + resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");
            }

            using var stream = assembly.GetManifestResourceStream(resourcePath);
            using var reader = new StreamReader(stream ?? throw new Exception("Incorrect Prompt File"), Encoding.UTF8);
            return reader.ReadToEnd();
        }

        public GptClient GetClient()
        {
            return new GptClient(RezApi.Settings.OpenAiApiKey);
        }

        /// <summary>
        /// using Customer's own API key
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public GptClient GetClient(User user)
        {
            throw new NotImplementedException();
        }
    }
}
