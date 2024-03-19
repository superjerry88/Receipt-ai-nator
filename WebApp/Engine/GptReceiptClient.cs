using Newtonsoft.Json;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using System.Reflection;
using System.Text;
using WebApp.Model.Receipts;
using WebApp.Services;

namespace WebApp.Engine;

public class GptReceiptClient(string apiKey) : IJob
{
    public const string PromptFile = "OCRV001";

    private OpenAIService Client { get; } = new(new OpenAiOptions()
    {
        ApiKey = apiKey
    });

    public async Task<ScanResult> ExtractImage(string imagePath)
    {
        var imageBytes = await File.ReadAllBytesAsync(imagePath);
        var completionResult = await Client.ChatCompletion.CreateCompletion(
            new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem(GetPrompt(PromptFile)),
                    ChatMessage.FromUser(
                        new List<MessageContent>
                        {
                            MessageContent.ImageBinaryContent(imageBytes,
                                StaticValues.ImageStatics.ImageDetailTypes.Low)
                        }
                    ),
                },
                MaxTokens = 4096,
                Model = Models.Gpt_4_vision_preview,
                N = 1
            }
        );

        if (completionResult.Successful)
        {
            Console.WriteLine($"[USAGE] {completionResult.Usage.TotalTokens} Tokens");
            Console.WriteLine(completionResult.Choices.First().Message.Content);
        }

        await RezApi.DbManager.GptUsage.AddUsage(completionResult);

        var json = JsonConvert.SerializeObject(completionResult,Formatting.Indented);
        var filename = DateTime.Now.Ticks + "_" + Guid.NewGuid().ToString("N")[..6] + ".json";
        await File.WriteAllTextAsync(Path.Combine(RezApi.Files.ResponseFolderPath, filename), json);
        return JsonConvert.DeserializeObject<ScanResult>(json)!;
    }

    private string GetPrompt(string file)
    {
        var resourceName = "Data.Prompt." + file;
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
}