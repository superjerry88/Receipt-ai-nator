﻿using Newtonsoft.Json;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using System.Reflection;
using System.Text;
using WebApp.Model.Receipts;
using WebApp.Services;

namespace WebApp.Engine;

public class GptReceiptClient : IJob
{
    public const string PromptFile = "OCRV001";
    private bool IsSiteToken { get; }= false;

    public GptReceiptClient(string apiKey)
    {
        Client = new(new OpenAiOptions()
        {
            ApiKey = apiKey,
        });
        IsSiteToken = apiKey == RezApi.Settings.OpenAiApiKey;
    }

    private OpenAIService Client { get; }

    public static async Task<bool> ValidateOpenAiKey(string apiKey)
    {
        try
        {
            var client = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = apiKey
            });
            await client.Models.ListModel();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

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
                Model = Models.Gpt_4o_2024_05_13,
                N = 1
            }
        );

        var json = "";
        if (completionResult.Successful)
        {
            json = completionResult.Choices.First().Message.Content;
            Console.WriteLine($"[USAGE] {completionResult.Usage.TotalTokens} Tokens");
            Console.WriteLine(json);
        }

        if (completionResult.Id != null)
        {
            await RezApi.DbManager.GptUsage.AddUsage(completionResult);
        }
        
        var filename = "Res" + DateTime.Now.Ticks + "_" + Guid.NewGuid().ToString("N")[..6] + ".json";
        await File.WriteAllTextAsync(Path.Combine(RezApi.Files.ResponseFolderPath, filename), json);
        var scanResult = JsonConvert.DeserializeObject<ScanResult>(json)!;

        scanResult.TokenConsumed = completionResult.Usage.TotalTokens;
        scanResult.UsingSiteToken = IsSiteToken;

        return scanResult;
    }

    public static string GetPrompt(string file)
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