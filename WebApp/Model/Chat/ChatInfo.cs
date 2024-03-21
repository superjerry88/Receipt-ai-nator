namespace WebApp.Model.Chat;

public class ChatInfo
{
    public string UserId { get; set; } = "";
    public string Session { get; set; } = "";
    public string Message { get; set; } = "";
    public bool IsTyping { get; set; }
    public ChatType Type { get; set; }
    public DateTime MessageDateTime { get; set; }

    public static ChatInfo GetSystemChat(string userId, string session, string message)
    {
        return new ChatInfo
        {
            UserId = userId,
            Session = session,
            Message = message,
            Type = ChatType.System,
            MessageDateTime = DateTime.Now,
            IsTyping = false
        };
    }

    public static ChatInfo GetUserChat(string userId, string session, string message)
    {
        return new ChatInfo
        {
            UserId = userId,
            Session = session,
            Message = message,
            Type = ChatType.User,
            MessageDateTime = DateTime.Now,
            IsTyping = false
        };
    }

}