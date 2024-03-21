using WebApp.Engine;

namespace WebApp.Services
{
    public class ChatService
    {
        private CurrentSession _currentSession;

        public ChatService(CurrentSession currentSession)
        {
            _currentSession = currentSession;
        }

        public async Task<GptChatClient> GetChatClient()
        {
            var user = await _currentSession.GetUser();
            return new GptChatClient(user);
        }
    }
}
