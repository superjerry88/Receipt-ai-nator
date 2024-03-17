using Newtonsoft.Json;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using WebApp.DB;
using WebApp.Services;

namespace WebApp.Engine;

public class GptClient(string apiKey)
{
    private OpenAIService Client { get; } = new(new OpenAiOptions()
    {
        ApiKey = apiKey
    });

    public async Task Try(string imagePath)
    {
        //todo: temporary code to not consume my precious token while testing
        if (RezApi.Settings.UseMock)
        {
            Console.WriteLine("MOCK MODE DETECTED... NOTHING HAPPENS FOR NOW");
            return;
        }
        var imageBytes = await File.ReadAllBytesAsync(imagePath);
        var completionResult = await Client.ChatCompletion.CreateCompletion(
            new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem(RezApi.AiManager.GetPrompt(PromptList.Ocr)),
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

        await RezApi.DbManager.Get<DbGptUsage>().AddUsage(completionResult);

        var json = JsonConvert.SerializeObject(completionResult,Formatting.Indented);
        var filename = DateTime.Now.Ticks + "_" + Guid.NewGuid().ToString("N")[..6] + ".json";
        await File.WriteAllTextAsync(Path.Combine(RezApi.Files.ResponseFolderPath, filename), json);
    }
}