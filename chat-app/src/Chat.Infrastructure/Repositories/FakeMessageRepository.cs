using Chat.Core.Model;
using Chat.Core.Repostories;

namespace Chat.Infrastructure.Repositories;

internal class FakeMessageRepository : IMessagesRepository
{
    private readonly List<ChatMessage> _messages = new List<ChatMessage>();
    public Task AddMessage(ChatMessage message)
    {
        _messages.Add(message);
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<ChatMessage> GetAllMessages()
    {
        await Task.Yield();
        foreach (var message in _messages)
        {
            yield return message;
        }
    }
}