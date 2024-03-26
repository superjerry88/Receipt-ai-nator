using Newtonsoft.Json;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.Tokenizer.GPT3;
using WebApp.Model;
using WebApp.Model.Chat;
using WebApp.Model.Receipts;
using WebApp.Services;

namespace WebApp.Engine;

public class GptChatClient
{
    private User User { get; set; }
    public string UserId { get; set; }
    public string SessionId { get; set; } = Guid.NewGuid().ToString("N");
    public bool IsTyping { get; set; } = false;
    public List<ChatInfo> Chats { get; set; } = new List<ChatInfo>();

    public event EventHandler<ChatInfo> NewChat = null!;
    public event EventHandler<bool> IsClientTyping = null!;
    public event EventHandler MsgUpdated = null!;

    private OpenAIService Client { get; }

    private bool IsUsingUsersToken { get; set; }
    private string UserHistoryJson { get; set; }
    private ChatCompletionCreateRequest Prompt { get; set; }

    public string StartingChat = "Welcome to Rezeipt, ask anything you want regarding your recent purchases!";

    public GptChatClient(User? user)
    {
        if (user == null)
        {
            throw new Exception("User not found");
        }

        UserId = user.Id;
        User = user;

        if (user.HasOpenAiKey)
        {
            Client = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = user.GetApiKey()
            });
            IsUsingUsersToken = true;
        }
        else
        {
            Client = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = RezApi.Settings.OpenAiApiKey
            });
            IsUsingUsersToken = false;
        }

        //Get the latest 10 simplified receipts based on current user
        var receipts = RezApi.Jobs
            .GetJobsByUser(user)
            .Where(scan => scan is { IsCompleted: true, Result: not null })
            .Select(scan => scan.Result)
            .Where(scan => scan is { Receipts.Count: > 0 })
            .SelectMany(scan => scan?.Receipts)
            .OrderByDescending(receipt => receipt.DateTime)
            .Take(10)
            .Select(x => new SimpleReceipt(x))
            .ToList();

        UserHistoryJson = JsonConvert.SerializeObject(receipts);

        var system = GptReceiptClient.GetPrompt("CHAT001") + UserHistoryJson;
        Prompt = new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem(system)
            },
            Model = Models.Gpt_4_turbo_preview,
            N = 1
        };
        Chats.Add(ChatInfo.GetSystemChat(UserId, SessionId, StartingChat));
    }

    public bool HasEnoughToken()
    {
        if (IsUsingUsersToken) return true;
        var user = RezApi.Users.GetUser(UserId);
        return user?.FreeTokenBalance > 0;
    }

    public async Task AddQuestion(string question)
    {

        //Set Question 
        AddChat(ChatInfo.GetUserChat(UserId, SessionId, question));
        SetTyping(true);

        //Set response placeholder
        var msgResponse = ChatInfo.GetSystemChat(UserId, SessionId, "");
        AddChat(msgResponse);

        //Get response
        Prompt.Messages.Add(ChatMessage.FromUser(question));
        var gptResponse = await Client.ChatCompletion.CreateCompletion(Prompt);
        msgResponse.Message = gptResponse.Choices.First().Message.Content!;
        Prompt.Messages.Add(ChatMessage.FromSystem(msgResponse.Message));

        //Data collection and token consumption
        await RezApi.DbManager.GptUsage.AddUsage(gptResponse);
        if (!IsUsingUsersToken)
        {
            await RezApi.Users.ConsumeToken(UserId, gptResponse.Usage.TotalTokens);
        }
        Console.WriteLine($"Used: {gptResponse.Usage.TotalTokens}");

        //Set response as complete
        SetTyping(false);
    }

    public async Task AddQuestionStream(string question)
    {

        //Set Question 
        AddChat(ChatInfo.GetUserChat(UserId, SessionId, question));
        SetTyping(true);

        //Set response placeholder
        var msgResponse = ChatInfo.GetSystemChat(UserId, SessionId, "");
        AddChat(msgResponse);

        //why does this line fixed the visual rendering ??? WHY ???? 
        await Task.Delay(10);
        
        //Get response
        Prompt.Messages.Add(ChatMessage.FromUser(question));

        var fullMessage = "";
        var gptResponse =  Client.ChatCompletion.CreateCompletionAsStream(Prompt);
        
        await foreach (var completion in gptResponse)
        {
            if (completion.Successful)
            {
                fullMessage += completion.Choices.FirstOrDefault()?.Message.Content;
                msgResponse.Message = fullMessage;
                MsgUpdated?.Invoke(this,null!);
            }
            else
            {
                if (completion.Error == null)
                {
                    throw new Exception("Unknown Error");
                }

                Console.WriteLine($"{completion.Error.Code}: {completion.Error.Message}");
            }
        }

        Prompt.Messages.Add(ChatMessage.FromSystem(msgResponse.Message));

        var allText = string.Join(".", Prompt.Messages.Select(x => x.Content));
        var tokens = TokenizerGpt3.TokenCount(allText, true);
        Console.WriteLine($"Used: {tokens} tokens");

        if (!IsUsingUsersToken)
        {
            await RezApi.Users.ConsumeToken(UserId, tokens);
        }
        //Set response as complete
        SetTyping(false);
    }

    private void AddChat(ChatInfo chat)
    {
        Chats.Add(chat);
        NewChat?.Invoke(this, chat);
    }

    private void SetTyping(bool typing)
    {
        IsTyping = typing;
        IsClientTyping?.Invoke(this, IsTyping);
    }
}