using Chat.Core.Model;
using Chat.Core.Repostories;

namespace Chat.Infrastructure.Repositories;

internal class FakeMessageRepository : IMessagesRepository
{
    private readonly List<ChatMessage> _messages = new List<ChatMessage>
        {
            new ChatMessage (Guid.NewGuid(), "janpawel3", "xDD",  DateTime.Now.AddDays(-2) ),
            new ChatMessage (Guid.NewGuid(), "papaj", "xD3D",  DateTime.Now.AddDays(-3) ),
            new ChatMessage (Guid.NewGuid(), "karolwojtylak", "xDD2",  DateTime.Now.AddDays(-1) ),
        };
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