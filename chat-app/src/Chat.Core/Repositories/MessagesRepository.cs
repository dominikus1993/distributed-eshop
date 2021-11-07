using Chat.Core.Model;

namespace Chat.Core.Repostories;

public interface IMessagesRepository
{
    Task AddMessage(ChatMessage message);
    IAsyncEnumerable<ChatMessage> GetAllMessages();
}